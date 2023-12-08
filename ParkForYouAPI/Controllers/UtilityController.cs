using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ParkForYouAPI.APIModels;

namespace ParkForYouAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController : ControllerBase
    {
        public UtilityController()
        {

        }

        [HttpGet]
        [Route("load-languages")]
        public IActionResult LoadLanguages()
        {
            BasicResponse basicResponse = new BasicResponse();
            string languageCode = string.Empty;
            string path = _GetFilePath(languageCode);
            using (StreamReader streamReader = new StreamReader(path))
            {
                string json = streamReader.ReadToEnd();
                Dictionary<object, string> languages = JsonConvert.DeserializeObject<Dictionary<object, string>>(json);
                basicResponse.Data = true;
            }
            return Ok(basicResponse);
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
    }
}