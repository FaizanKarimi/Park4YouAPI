using System;
using System.Data;
using System.Text;
using BackgroundSchedulers;
using BusinessOperations.Implementations;
using BusinessOperations.Interfaces;
using Components.Identity;
using Components.Services.Implementation;
using Components.Services.Interfaces;
using Components.Services.OneSignal;
using Components.Services.PRS;
using Components.Services.QuickPay;
using Components.Services.Solvision;
using Infrastructure.Cache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSwag.AspNetCore;
using ParkForYouAPI.Middlewares;
using Repository.Implementations;
using Repository.Interfaces;
using Repository.Provider;
using Services.Implementations;
using Services.Interfaces;

namespace ParkForYouAPI
{
    public class Startup
    {
        #region Private Members
        private const string _appSettingKey = "AppSettings";
        private const string _connectionStringName = "ParkForYouADO";
        private const string _apiVersion = "v1";
        private const string _apiTitle = "Park4You";
        private const string _apiDescription = "Park4You API";
        #endregion

        #region Public Properties
        public IConfiguration Configuration { get; }
        #endregion

        #region Constructor
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        #region Public Methods
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwagger();
            services.AddMemoryCache();

            this._RegisterApplicationServices(services);
            this._RegisterComponents(services);
            this._RegisterSingleInstances(services);
            this._RegisterRepositories(services);
            this._RegisterServices(services);
            this._RegisterBusinessOperations(services);

            ServiceProvider servivceProvider = services.BuildServiceProvider();
            IBackgroundJobService backgroundJobService = servivceProvider.GetService<IBackgroundJobService>();
        }

        public void Configure(IApplicationBuilder application, IApplicationLifetime applicationLifeTime, IHostingEnvironment enviroment)
        {
            ILogging logger = application.ApplicationServices.GetService<ILogging>();

            if (enviroment.IsDevelopment())
            {
                application.UseDeveloperExceptionPage();
            }

            application.UseCors(builder => builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials());

            application.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Version = _apiVersion;
                    document.Info.Title = _apiTitle;
                    document.Info.Description = _apiDescription;
                };
            });

            application.UseAuthentication();
            application.UseCustomMessageHandler();
            application.UseMvc();
        }
        #endregion

        #region Private Methods
        private void _RegisterApplicationServices(IServiceCollection services)
        {
            #region Identity
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString(_connectionStringName)));
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();
            #endregion

            #region AppSettings
            services.AddOptions<AppSettings>();
            services.Configure<AppSettings>(Configuration.GetSection(_appSettingKey));
            #endregion

            #region Authentication Services
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration.GetSection(_appSettingKey).Get<AppSettings>().JwtIssuer,
                            ValidAudience = Configuration.GetSection(_appSettingKey).Get<AppSettings>().JwtIssuer,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection(_appSettingKey).Get<AppSettings>().JwtKey)),
                            ClockSkew = TimeSpan.Zero
                        };
                    });
            services.AddAuthorization(authorization =>
            {
                authorization.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
            #endregion

            #region HttpContextAccessor (Session)
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            #endregion                        
        }

        private void _RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IDbConnection, MSSQL>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IUserSettingRepository, UserSettingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IUserCardRepository, UserCardRepository>();
            services.AddScoped<IUserAreaRepository, UserAreaRepository>();
            services.AddScoped<IParkingRepository, ParkingRepository>();
            services.AddScoped<IParkingLotRepository, ParkingLotRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IChargeSheetRepository, ChargeSheetRepository>();
            services.AddScoped<IChargeSheetPriceRepository, ChargeSheetPriceRepository>();
            services.AddScoped<IPaymentOrderRepository, PaymentOrderRepository>();
            services.AddScoped<IParkingNotificationRepository, ParkingNotificationRepository>();
        }

        private void _RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IUserSettingService, UserSettingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IUserCardService, UserCardService>();
            services.AddScoped<IUserAreaService, UserAreaService>();
            services.AddScoped<IParkingService, ParkingService>();
            services.AddScoped<IParkingLotService, ParkingLotService>();
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IChargeSheetService, ChargeSheetService>();
        }

        private void _RegisterBusinessOperations(IServiceCollection services)
        {
            services.AddScoped<IBOVehicles, BOVehicles>();
            services.AddScoped<IBOUserSetting, BOUserSetting>();
            services.AddScoped<IBOUserCard, BOUserCard>();
            services.AddScoped<IBOUserArea, BOUserArea>();
            services.AddScoped<IBOUser, BOUser>();
            services.AddScoped<IBOParkingLot, BOParkingLot>();
            services.AddScoped<IBOParking, BOParking>();
            services.AddScoped<IBODevice, BODevice>();
            services.AddScoped<IBOChargeSheet, BOChargeSheet>();
            services.AddScoped<IBOUserProfile, BOUserProfile>();
        }

        private void _RegisterComponents(IServiceCollection services)
        {
            services.AddTransient<ITwillioService, TwillioService>();
            services.AddTransient<IQuickPayService, QuickPayService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<ISolvisionService, SolvisionService>();
            services.AddTransient<IPushNotificationService, PushNotificationService>();
            services.AddTransient<IPRSService, PRSService>();
        }

        private void _RegisterSingleInstances(IServiceCollection services)
        {
            services.AddSingleton<ILogging, Logging>();
            services.AddSingleton<IBackgroundJobService, BackgroundJobService>();
        }
        #endregion
    }
}