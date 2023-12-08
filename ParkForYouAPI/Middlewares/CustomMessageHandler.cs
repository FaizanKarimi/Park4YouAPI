using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Components.Services.Interfaces;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ParkForYouAPI.APIModels;

namespace ParkForYouAPI.Middlewares
{
    public class CustomMessageHandler
    {
        #region Private Members
        private readonly RequestDelegate _next;
        private readonly ILogging _logger;
        private readonly IEmailService _emailService;
        #endregion

        #region Constructor
        public CustomMessageHandler(RequestDelegate next, ILogging logging, IEmailService emailService)
        {
            _next = next;
            _logger = logging;
            _emailService = emailService;
        }
        #endregion

        #region Public Methods
        public async Task Invoke(HttpContext context)
        {
            try
            {
                Stream originalBody = context.Response.Body;
                try
                {
                    string responseBody = null;
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        context.Response.Body = memStream;
                        await _next.Invoke(context);
                        memStream.Position = 0;
                        responseBody = new StreamReader(memStream).ReadToEnd();
                    }
                    BasicResponse data = JsonConvert.DeserializeObject<BasicResponse>(responseBody);
                    string obj = JsonConvert.SerializeObject(data);
                    byte[] buffer = Encoding.UTF8.GetBytes(obj);
                    using (MemoryStream output = new MemoryStream(buffer))
                    {
                        await output.CopyToAsync(originalBody);
                    }
                }
                finally
                {
                    context.Response.Body = originalBody;
                }
            }
            catch (Exception ex)
            {
                await _HandleExceptionAsync(context, ex);
                return;
            }
        }
        #endregion

        #region Private Methods
        private async Task _HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string errorMessage = string.Empty;
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            if (exception.GetType() == typeof(Park4YouException))
            {
                httpStatusCode = HttpStatusCode.BadRequest;
                errorMessage = _GetLanguageValue(exception.Message);
                _logger.Debug(string.Format("Exception is: {0}", exception));
            }
            else
            {
                _logger.Debug(string.Format("Exception is: {0}", exception));
                errorMessage = _GetLanguageValue(ErrorMessages.INTERNAL_SERVER_ERROR.ToString());
                _emailService.SendErrorEmail(Constants.CTOEmailAddress, errorMessage);
            }

            BasicResponse basicResponse = new BasicResponse()
            {
                Data = null,
                Success = false,
                ErrorMessasge = errorMessage
            };
            string result = JsonConvert.SerializeObject(basicResponse);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)httpStatusCode;
            await context.Response.WriteAsync(result);
        }

        private string _GetLanguageValue(string key)
        {
            string languageValue = string.Empty;
            string languageCode = string.Empty;
            string path = _GetFilePath(languageCode);
            using (StreamReader streamReader = new StreamReader(path))
            {
                string json = streamReader.ReadToEnd();
                Dictionary<object, string> languages = JsonConvert.DeserializeObject<Dictionary<object, string>>(json);
                languages.TryGetValue(key, out languageValue);
            }
            return languageValue;
        }

        private string _GetFilePath(string languageCode)
        {
            string path = string.Empty;
            string language = _GetUserLanguageFile(languageCode);
            if (!string.IsNullOrEmpty(language))
                path = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\Languages\\", language.ToLower(), ".json");
            else
                path = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\Languages\\", "en-us", ".json");

            return path;
        }

        private string _GetUserLanguageFile(string userLanguage)
        {
            string language = string.Empty;
            switch (userLanguage)
            {
                case "de":  //Germany
                    language = "de-ger";
                    break;
                case "da":  //Danish
                    language = "en-us";
                    break;
                case "sv":  //Swedish
                    language = "sv-sw";
                    break;
                case "nor":  //Norwegian
                    language = "en-us";
                    break;
                case "it":  //Italian
                    language = "it-it";
                    break;
                case "es":  //Spanish
                    language = "es-spa";
                    break;
                case "fr":  //French
                    language = "en-us";
                    break;
                default:    //English
                    language = "en-us";
                    break;
            }
            return language;
        }
        #endregion
    }
}