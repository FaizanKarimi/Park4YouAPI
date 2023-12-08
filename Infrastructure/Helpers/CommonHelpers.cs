using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Infrastructure.Helpers
{
    public class CommonHelpers
    {
        /// <summary>
        /// Generate the random number and return's in string format.
        /// </summary>
        /// <returns></returns>
        public static string GenrateRandomNumber()
        {
            Random r = new Random();
            int number = r.Next(10000, 90000);
            int finalnumber = 0;
            for (int i = 0; i < Convert.ToString(number).Length; i++)
            {
                Random rn = new Random();
                finalnumber = r.Next(number, 99999);
            }
            return finalnumber.ToString();
        }

        /// <summary>
        /// Get card expiration month.
        /// </summary>
        /// <param name="cardExpiry"></param>
        /// <returns></returns>
        public static string GetCardExpirationMonth(string cardExpiry)
        {
            string[] splitExpireDate = cardExpiry.Split('/');
            string month = splitExpireDate.First();
            return month;
        }

        /// <summary>
        /// Get card expiration year.
        /// </summary>
        /// <param name="cardExpiry"></param>
        /// <returns></returns>
        public static string GetCardExpirationYear(string cardExpiry)
        {
            string[] splitExpireDate = cardExpiry.Split('/');
            string year = splitExpireDate.Last().Substring(2, 2);
            return year;
        }

        /// <summary>
        /// Generate the new order id.
        /// </summary>
        /// <returns></returns>
        public static string GetOrderId()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 19);
        }

        /// <summary>
        /// Generate the new random number.
        /// </summary>
        /// <returns></returns>
        public static string GetRandomNumber()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 19);
        }

        /// <summary>
        /// Get the domain name of the application.
        /// </summary>
        /// <returns></returns>
        public static string GetDomainName()
        {
            var baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return baseDirectory;
        }

        /// <summary>
        /// Get the total minutes difference between stoptime and starttime.
        /// </summary>
        /// <param name="stopTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static double GetTotalMinutes(DateTime stopTime, DateTime startTime)
        {
            double minutes = 0;
            TimeSpan span = (stopTime - startTime);
            minutes = span.TotalMinutes;
            return minutes;
        }

        /// <summary>
        /// Get the total milliseconds difference between endTime and startTime.
        /// </summary>
        /// <param name="endTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static double GetTotalMilliseconds(DateTime endTime, DateTime startTime)
        {
            TimeSpan timespan = endTime - startTime;
            return timespan.TotalMilliseconds;
        }

        /// <summary>
        /// Get the total seconds between stoptime and starttime.
        /// </summary>
        /// <param name="stopTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static int GetTotalSeconds(DateTime stopTime, DateTime startTime)
        {
            int seconds = 0;
            TimeSpan span = (stopTime - startTime);
            seconds = Convert.ToInt32(span.TotalSeconds);
            return seconds;

        }

        /// <summary>
        /// Get the total seconds difference between starttime and endtime.
        /// </summary>
        /// <param name="stopTime"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public static int GetSecondsDifference(DateTime stopTime, DateTime startTime)
        {
            int seconds = 0;
            TimeSpan span = (stopTime - startTime);
            TimeSpan ticks = TimeSpan.FromTicks(span.Ticks);
            seconds = Convert.ToInt32(ticks.Seconds);
            return seconds;
        }

        /// <summary>
        /// Returns true if the try parse is successfull.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParseEnum<TEnum>(string value) where TEnum : struct
        {
            bool IsParsed = false;
            TEnum obj;
            if (Enum.TryParse<TEnum>(value, out obj))
            {
                IsParsed = true;
            }
            return IsParsed;
        }

        /// <summary>
        /// Parse the enum value. Exception will be thrown if is unable to parse the enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Get the converted amount with respective country.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public static string ConvertedAmount(decimal value, string countryName)
        {
            string formattedAmount = string.Empty;
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            CultureInfo countryInfo = cultures.FirstOrDefault(cultureInfo => cultureInfo.EnglishName.Contains("Denmark"));
            if (countryInfo != null)
            {
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(countryInfo.Name);
                formattedAmount = value.ToString("C", new CultureInfo(cultureInfo.Name));
            }            
            return formattedAmount;
        }

        /// <summary>
        /// Get the price with the country price formation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public static string FormattedPriceToDecimal(string value)
        {
            double amount = Convert.ToDouble(value);
            string formattedAmount = amount.ToString("N2");
            return formattedAmount;
        }

        /// <summary>
        /// Get the vendor index saved in the appsettings.json file
        /// </summary>
        /// <param name="userIds"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetVendorIndex(string userIds, string userId)
        {
            List<string> array = userIds.Split(',').ToList();
            int index = array.FindIndex(x => x.Trim().Equals(userId));
            return index;
        }
    }
}