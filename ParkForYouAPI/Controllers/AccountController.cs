using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessOperations.Interfaces;
using Components.Identity;
using Components.Services.Interfaces;
using Infrastructure.Cache;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkForYouAPI.APIModels;
using ParkForYouAPI.APIRequestModels.User;

namespace ParkForYouAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Private Members
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBOUser _bOUser;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IConfiguration _config;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ILogging _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBODevice _bODevice;
        private readonly IBOParking _bOParking;
        #endregion

        #region Constructor
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IBOUser bOUser,
            IConfiguration configuaration, IOptions<AppSettings> appSettings, ILogging logging, IHttpContextAccessor httpContextAccessor, IBODevice bODevice, IBOParking bOParking)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _bOUser = bOUser;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _config = configuaration;
            _appSettings = appSettings;
            _logger = logging;
            _httpContextAccessor = httpContextAccessor;
            _bODevice = bODevice;
            _bOParking = bOParking;
        }
        #endregion

        #region API'S
        [Authorize]
        [HttpPost]
        [Route("create-admin-user")]
        public async Task<IActionResult> CreateAdminUserAsync()
        {
            string password = "Adm!np@$$Park4u";
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = new ApplicationUser()
            {
                UserName = "info@park4youserver.com",
                Email = "info@park4youserver.com",
                DateRegistration = DateTime.UtcNow,
                EmailConfirmed = true
            };
            IdentityResult result = await this._userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                result = await this._userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
                if (result.Succeeded)
                    basicResponse.Data = true;
            }
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            _logger.Debug("Getting user by mobileNumber.");
            ApplicationUser dbUser = await this._userManager.FindByNameAsync(registerRequest.MobileNumber);
            if (dbUser == null)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = registerRequest.MobileNumber,
                    Email = registerRequest.Email,
                    MobileNumber = registerRequest.MobileNumber,
                    DeviceToken = registerRequest.DeviceToken,
                    CountryCode = registerRequest.CountryCode,
                    DateRegistration = DateTime.UtcNow,
                    PhoneNumber = registerRequest.MobileNumber
                };
                _logger.Debug("Creating new user in system.");
                IdentityResult result = await this._userManager.CreateAsync(user, registerRequest.Password);
                if (result.Succeeded)
                {
                    _logger.Debug(string.Format("Adding user to role."));
                    IdentityResult roleResult = await this._userManager.AddToRoleAsync(user, UserRoles.User.ToString());
                    if (roleResult.Succeeded)
                    {
                        ApplicationUser newUser = await this._userManager.FindByNameAsync(registerRequest.MobileNumber);
                        _logger.Debug(string.Format("Registering the new user in the system."));
                        basicResponse.Data = _bOUser.RegisterUser(registerRequest.CountryCode, registerRequest.MobileNumber, registerRequest.Country, registerRequest.FirstName, registerRequest.LastName, string.Empty, newUser.Id, registerRequest.DeviceToken, registerRequest.RegistrationToken, (int)registerRequest.DeviceTypeId, registerRequest.Email, registerRequest.Language);
                    }
                    else
                        throw new Park4YouException(ErrorMessages.USER_NOT_ADDED_TO_ROLE);
                }
                else
                    throw new Park4YouException(ErrorMessages.USER_NOT_CREATED);
            }
            else
                throw new Park4YouException(ErrorMessages.USER_ALREADY_EXIST);

            return Ok(basicResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("create-role")]
        public async Task<IActionResult> CreateRole(AddRoleRequest addRoleRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            bool isExist = await this._roleManager.RoleExistsAsync(addRoleRequest.RoleName);
            if (!isExist)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = addRoleRequest.RoleName
                };
                IdentityResult result = await this._roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    basicResponse.Data = true;
                }
                else
                {
                    if (result.Errors != null)
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            throw new Park4YouException(error.Description);
                        }
                    }
                }
            }
            else
                throw new Park4YouException(ErrorMessages.ROLE_ALREADY_EXIST);

            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("account-verification")]
        public async Task<IActionResult> AccountVerification(VerifyAccountRequest verifyAccountRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = await this._userManager.FindByNameAsync(verifyAccountRequest.MobileNumber);
            if (user != null)
                basicResponse.Data = _bOUser.VerifyAccount(verifyAccountRequest.MobileNumber, verifyAccountRequest.VerificationCode);
            else
                throw new Park4YouException(ErrorMessages.USER_INVALID_USERNAME_PASSWORD);

            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("resend-code")]
        public async Task<IActionResult> ResendCode(ResendCodeRequest resendCodeRequest)
        {
            _logger.Debug(string.Format("Resend code process started."));
            string email = _httpContextAccessor.GetCurrentUserEmail();
            _logger.Debug("Getting user by mobileNumber.", email);
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = await this._userManager.FindByNameAsync(resendCodeRequest.MobileNumber);
            if (user != null)
                basicResponse.Data = _bOUser.UpdateVerificationCode(resendCodeRequest.MobileNumber);
            else
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);

            _logger.Debug(string.Format("Resend code process ended."));

            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            _logger.Debug("Reset password process started.");
            BasicResponse basicResponse = new BasicResponse();
            _logger.Debug(string.Format("Getting user by mobileNumber: {0}", resetPasswordRequest.MobileNumber));
            ApplicationUser user = await this._userManager.FindByNameAsync(resetPasswordRequest.MobileNumber);
            if (user != null)
            {
                _logger.Debug(string.Format("Changing the current password with new password of the user: {0}", user.UserName));
                IdentityResult result = await this._userManager.ChangePasswordAsync(user, resetPasswordRequest.CurrentPassword, resetPasswordRequest.NewPassword);
                if (result.Succeeded)
                {
                    IdentityResult removedResult = await this._userManager.RemovePasswordAsync(user);
                    if (removedResult.Succeeded)
                    {
                        result = await this._userManager.AddPasswordAsync(user, resetPasswordRequest.NewPassword);
                        if (result.Succeeded)
                        {
                            basicResponse.Data = true;
                        }
                        else
                        {
                            if (result.Errors != null)
                            {
                                foreach (IdentityError error in result.Errors)
                                {
                                    throw new Park4YouException(error.Description);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (removedResult.Errors != null)
                        {
                            foreach (IdentityError error in removedResult.Errors)
                            {
                                throw new Park4YouException(error.Description);
                            }
                        }
                    }
                }
                else
                    throw new Park4YouException(ErrorMessages.USER_INVALID_CURRENT_PASSWORD);
            }
            else
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);

            _logger.Debug("Reset password process ended.");

            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            _logger.Debug("Change password process started.");
            BasicResponse basicResponse = new BasicResponse();
            _logger.Debug(string.Format("Getting user by mobileNumber: {0}", changePasswordRequest.MobileNumber));
            ApplicationUser user = await this._userManager.FindByNameAsync(changePasswordRequest.MobileNumber);
            if (user != null)
            {
                _logger.Debug(string.Format("Removing the password of the user: {0}", user.UserName));
                IdentityResult removedResult = await this._userManager.RemovePasswordAsync(user);
                if (removedResult.Succeeded)
                {
                    _logger.Debug(string.Format("Adding new password of the user: {0}", user.UserName));
                    IdentityResult result = await this._userManager.AddPasswordAsync(user, changePasswordRequest.NewPassword);
                    if (result.Succeeded)
                    {
                        basicResponse.Data = true;
                    }
                    else
                    {
                        if (result.Errors != null)
                        {
                            foreach (IdentityError error in result.Errors)
                            {
                                throw new Park4YouException(error.Description);
                            }
                        }
                    }
                }
                else
                {
                    if (removedResult.Errors != null)
                    {
                        foreach (IdentityError error in removedResult.Errors)
                        {
                            throw new Park4YouException(error.Description);
                        }
                    }
                }
            }
            else
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);

            _logger.Debug("Change password process ended.");

            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("user-login")]
        public async Task<IActionResult> UserLogin(UserLoginRequest userLoginRequest)
        {
            _logger.Debug("User login process started.");
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = await this._userManager.FindByNameAsync(userLoginRequest.Username);
            if (user != null)
            {
                var verifiedUser = await _signInManager.PasswordSignInAsync(userLoginRequest.Username, userLoginRequest.Password, false, false);
                if (verifiedUser.Succeeded)
                {
                    if (user.PhoneNumberConfirmed)
                    {
                        user.DeviceToken = userLoginRequest.DeviceToken;
                        _logger.Debug(string.Format("Updating the user with the mobileNumber: {0}", userLoginRequest.Username));
                        IdentityResult result = await this._userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            _bODevice.Update(userLoginRequest.DeviceToken, userLoginRequest.RegistrationToken, userLoginRequest.Username);
                            basicResponse.Data = true;
                        }
                    }
                    else
                        throw new Park4YouException(ErrorMessages.USER_NOT_SUCCESSFULLY_REGISTERED);
                }
                else
                    throw new Park4YouException(ErrorMessages.USER_INVALID_USERNAME_PASSWORD);
            }
            else
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);

            _logger.Debug("User login process ended.");

            return Ok(basicResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("delete-account")]
        public async Task<IActionResult> DeleteAccountAsync(DeleteAccountRequest deleteAccountRequest)
        {
            BasicResponse basicResponse = new BasicResponse();
            ApplicationUser user = await this._userManager.FindByNameAsync(deleteAccountRequest.MobileNumber);
            if (user != null)
            {
                user.UserName = string.Concat("anonymous", CommonHelpers.GetRandomNumber());
                user.Email = string.Concat("anonymous", CommonHelpers.GetRandomNumber());
                user.PhoneNumber = string.Concat("anonymous", CommonHelpers.GetRandomNumber());
                user.NormalizedUserName = string.Concat("anonymous", CommonHelpers.GetRandomNumber());
                await this._userManager.UpdateAsync(user);
                basicResponse.Data = _bOUser.DeleteAccount(deleteAccountRequest.MobileNumber);
            }
            else
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);
            return Ok(basicResponse);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            _logger.Debug(string.Format("Login process started."));
            BasicResponse basicResponse = new BasicResponse();
            _logger.Debug(string.Format("Getting user with the username: {0}", loginRequest.UserName));
            ApplicationUser user = await this._userManager.FindByNameAsync(loginRequest.UserName);
            if (user != null)
            {
                _logger.Debug(string.Format("Password sign for the user with the username: {0}", loginRequest.UserName));                
                var result = await _signInManager.PasswordSignInAsync(loginRequest.UserName, loginRequest.Password, false, false);
                if (result.Succeeded)
                {
                    _logger.Debug(string.Format("Getting roles of the user."));
                    IList<string> roles = await this._userManager.GetRolesAsync(user);
                    _logger.Debug(string.Format("Generating token of user with the username: {0} and role: {1}", user.UserName, roles.FirstOrDefault()));
                    basicResponse.Data = _GenerateJSONWebToken(user, roles);
                }
                else
                    throw new Park4YouException(ErrorMessages.USER_INVALID_USERNAME_PASSWORD);
            }
            else
            {
                throw new Park4YouException(ErrorMessages.USER_NOT_EXIST);
            }

            _logger.Debug(string.Format("Login process ended."));

            return Ok(basicResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogOutAsync()
        {
            _logger.Debug("Logout process started.");
            BasicResponse basicResponse = new BasicResponse();
            string userId = this._httpContextAccessor.GetCurrentUserId();
            _logger.Debug(string.Format("Getting user by id: {0}", userId));
            ApplicationUser user = await this._userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await this._signInManager.SignOutAsync();
            }
            _logger.Debug("Logout process ended.");
            return Ok(basicResponse);
        }
        #endregion

        #region Private Methods
        private TokenResponse _GenerateJSONWebToken(ApplicationUser applicationUser, IList<string> roles)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.JwtKey));
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, applicationUser.UserName),
                    new Claim(ClaimTypes.Email, applicationUser.Email),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(ClaimTypes.NameIdentifier , applicationUser.Id)
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_appSettings.Value.JwtExpireTime)),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
                IssuedAt = DateTime.UtcNow,
                Issuer = _appSettings.Value.JwtIssuer,
                Audience = _appSettings.Value.JwtIssuer
            };

            SecurityToken jwtToken = _jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            TokenResponse tokenResponse = new TokenResponse()
            {
                auth_token = token,
                expiration_time = jwtToken.ValidTo.ToString(Constants.MobileDateTimeFormat),
                issue_time = jwtToken.ValidFrom.ToString(Constants.MobileDateTimeFormat),
                role = roles.FirstOrDefault()
            };
            return tokenResponse;
        }
        #endregion
    }
}