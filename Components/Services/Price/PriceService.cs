using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.DataModels;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Newtonsoft.Json;

namespace Components.Services.Price
{
    #region OLD Formula
    public class PriceService
    {
        #region Private Members        
        private static int _one = 1;
        private static int _seven = 7;
        private static int _thirty = 30;
        private static int _hourMinutes = 60;

        private static decimal _basePrice = 00.0m;
        private static decimal _dayPrice = 0.0m;
        private static decimal _weekPrice = 0.0m;
        private static decimal _monthPrice = 0.0m;
        #endregion

        #region Public Methods
        public static decimal CalculatePrice(DateTime startTime, DateTime endTime, List<ChargeSheetPrices> ChargeSheetPrices)
        {
            decimal price = 0.0M;

            #region Time Calculation
            int days = TimeCalculationService.GetTotalDays(startTime, endTime);
            int hours = TimeCalculationService.GetRemainingHours(startTime, endTime);
            int minutes = TimeCalculationService.GetRemainingMinutes(startTime, endTime);
            int seconds = TimeCalculationService.GetRemainingSeconds(startTime, endTime);
            #endregion

            #region BasePrice, DailyPrice, WeekPrice, MonthPrice
            _basePrice = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString())).AttributeValue);
            _dayPrice = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.MAXIMUM_PRICE_DAILY_PRICE.ToString()))?.AttributeValue);
            _weekPrice = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.MAXIMUM_PRICE_WEEKLY_PRICE.ToString()))?.AttributeValue);
            _monthPrice = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.MAXIMUM_PRICE_MONTHLY_PRICE.ToString()))?.AttributeValue);
            #endregion

            #region Daily Price
            if (days >= _one && days < _seven)
                price = _GetDailyPrice(days, hours, minutes, seconds);
            #endregion

            #region Weekly Price 
            else if (days >= _seven && days < _thirty)
                price = _GetWeeklyPrice(days, hours, minutes, seconds);
            #endregion

            #region Monthly Price
            else if (days >= _thirty)
                price = _GetMonthlyPrice(days, hours, minutes, seconds);
            #endregion

            #region Base Price
            else
            {
                minutes = TimeCalculationService.GetTotalMinutes(startTime, endTime);
                price = (_basePrice / _hourMinutes) * minutes;
                if (_dayPrice > 0 && price > _dayPrice)
                    price = _dayPrice;
            }
            #endregion

            return price;
        }
        #endregion

        #region Private Methods
        private static decimal _GetDailyPrice(int days, int hours, int minutes, int seconds)
        {
            decimal price = 0.0m;
            decimal minutePrice = 0.0m;
            decimal hourPrice = 0.0m;

            if (minutes > 0)
            {
                minutes = seconds > 0 ? (minutes + 1) : minutes;
                minutePrice = (_basePrice / _hourMinutes) * minutes;
            }

            if (hours > 0)
                hourPrice = _basePrice * hours;

            decimal hourMinutePrice = hourPrice + minutePrice;
            if (hourMinutePrice > _dayPrice)
            {
                price = _dayPrice * (days + 1);
                if (price > _weekPrice)
                    price = _weekPrice;
            }
            else
                price = (_dayPrice * days) + hourPrice + minutePrice;

            if (price > _weekPrice)
                price = _weekPrice;

            return price;
        }

        private static decimal _GetWeeklyPrice(int days, int hours, int minutes, int seconds)
        {
            decimal price = 0.0m;
            int numberOfWeeks = Convert.ToInt32(days / 7);
            int numberOfDays = days % 7;

            decimal minutePrice = 0.0m;
            decimal hourPrice = 0.0m;
            decimal dayPrice = 0.0m;

            if (minutes > 0)
            {
                minutes = seconds > 0 ? (minutes + 1) : minutes;
                minutePrice = (_basePrice / _hourMinutes) * minutes;
            }

            if (hours > 0)
                hourPrice = _basePrice * hours;

            var hourMinutePrice = hourPrice + minutePrice;
            if (hourMinutePrice > _dayPrice)
            {
                hourPrice = _dayPrice;
                minutePrice = 0.0m;
            }

            //If the total hour is price is greater than daily price than it will fall in this condition
            if (hourPrice > _dayPrice)
                hourPrice = _dayPrice;

            if (numberOfDays > 0)
                dayPrice = _dayPrice * numberOfDays;

            decimal dayHourMinutePrice = dayPrice + hourPrice + minutePrice;
            if (dayHourMinutePrice > _weekPrice)
            {
                price = _weekPrice * (numberOfWeeks + 1);
                if (price > _monthPrice)
                    price = _monthPrice;
            }
            else
                price = (_weekPrice * numberOfWeeks) + dayPrice + hourPrice + minutePrice;

            if (price > _monthPrice)
                price = _monthPrice;

            return price;
        }

        private static decimal _GetMonthlyPrice(int days, int hours, int minutes, int seconds)
        {
            decimal price = 0.0m;
            int numberOfMonths = Convert.ToInt32(days / 30);
            int numberOfWeeks = (days - 30) / 7;
            int numberOfDays = (days - 30) % 7;

            decimal minutePrice = 0.0m;
            decimal hourPrice = 0.0m;
            decimal dayPrice = 0.0m;
            decimal weekPrice = 0.0m;

            if (minutes > 0)
            {
                minutes = seconds > 0 ? (minutes + 1) : minutes;
                minutePrice = (_basePrice / _hourMinutes) * minutes;
            }

            if (hours > 0)
                hourPrice = _basePrice * hours;

            if (numberOfDays > 0)
                dayPrice = _dayPrice * numberOfDays;

            if (numberOfWeeks > 0)
                weekPrice = _weekPrice * numberOfWeeks;

            decimal weekDayHourMinutePrice = weekPrice + dayPrice + hourPrice + minutePrice;
            if (weekDayHourMinutePrice > _monthPrice)
                price = _monthPrice * (numberOfMonths + 1);
            else
                price = (_monthPrice * numberOfMonths) + weekPrice + dayPrice + hourPrice + minutePrice;

            return price;
        }
        #endregion
    }
    #endregion

    //public class PriceService
    //{
    //    #region Public Methods
    //    public static decimal CalculatePrice(DateTime startDate, DateTime endDate, List<ChargeSheetPrices> ChargeSheetPrices)
    //    {
    //        startDate = _RemoveSeconds(startDate);
    //        endDate = _RemoveSeconds(endDate);
    //        List<CustomHours> hours = new List<CustomHours>();
    //        DateTime startHour = startDate;
    //        DateTime endHour;
    //        do
    //        {
    //            var mins = startHour.Minute;
    //            var minutes = 60 - mins;
    //            endHour = _AddMinutes(startHour, minutes);
    //            if (endHour <= endDate)
    //            {
    //                hours.Add(new CustomHours() { startHour = startHour, endHour = endHour, minutes = minutes });
    //            }
    //            else
    //            {
    //                hours.Add(new CustomHours() { startHour = startHour, endHour = endHour, minutes = (endDate.Minute - startHour.Minute) });
    //            }
    //            startHour = endHour;
    //        } while (startHour < endDate);

    //        int index = 0;
    //        foreach (CustomHours item in hours)
    //        {
    //            item.rate = _FindRateForHour(item.startHour, item.endHour, ChargeSheetPrices, index);
    //            decimal val = Convert.ToDecimal(item.minutes) / 60;
    //            item.rate = (item.rate * val);
    //            index++;
    //        }

    //        List<DayWiseRates> daysWise = _DayWiseRates(hours);
    //        daysWise = _ApplyDayWiseLimit(daysWise, ChargeSheetPrices);
    //        List<WeekWiseRates> weekWise = _WeekWiseRates(daysWise);
    //        weekWise = _ApplyWeekWiseLimit(weekWise, ChargeSheetPrices);
    //        List<MonthWiseRates> monthWise = _MonthWiseRates(weekWise);
    //        monthWise = _ApplyMonthWiseLimit(monthWise, ChargeSheetPrices);
    //        decimal totalRate = _TotalRate(monthWise);
    //        return totalRate;
    //    }
    //    #endregion

    //    #region Private Methods
    //    private static decimal _TotalRate(List<MonthWiseRates> monthWiseRates)
    //    {
    //        decimal sum = 0;
    //        foreach (MonthWiseRates item in monthWiseRates)
    //        {
    //            sum += item.sum;
    //        }
    //        return sum;
    //    }

    //    private static List<MonthWiseRates> _ApplyMonthWiseLimit(List<MonthWiseRates> monthWiseRates, List<ChargeSheetPrices> ChargeSheetPrices)
    //    {
    //        var monthLimit = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.MAXIMUM_PRICE_MONTHLY_PRICE.ToString())).AttributeValue);
    //        foreach (MonthWiseRates item in monthWiseRates)
    //        {
    //            if (item.sum > monthLimit)
    //            {
    //                item.sum = monthLimit;
    //            }
    //        }
    //        return monthWiseRates;
    //    }

    //    private static List<MonthWiseRates> _MonthWiseRates(List<WeekWiseRates> weekWiseRates)
    //    {
    //        List<MonthWiseRates> monthWiseRates = new List<MonthWiseRates>();
    //        for (int i = 0; i < weekWiseRates.Count; i++)
    //        {
    //            decimal number = i / 4;
    //            int monthIndex = Convert.ToInt32(Math.Floor(number));
    //            if (monthWiseRates.Exists(x => x.monthIndex == monthIndex))
    //            {
    //                MonthWiseRates item = monthWiseRates.FirstOrDefault(x => x.monthIndex == monthIndex);
    //                item.sum += weekWiseRates[i].sum;
    //            }
    //            else
    //            {
    //                monthWiseRates.Add(new MonthWiseRates() { monthIndex = monthIndex, sum = weekWiseRates[i].sum });
    //            }
    //        }
    //        return monthWiseRates;
    //    }

    //    private static List<WeekWiseRates> _ApplyWeekWiseLimit(List<WeekWiseRates> weekWiseRates, List<ChargeSheetPrices> ChargeSheetPrices)
    //    {
    //        decimal weekLimit = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.MAXIMUM_PRICE_WEEKLY_PRICE.ToString())).AttributeValue);
    //        foreach (WeekWiseRates item in weekWiseRates)
    //        {
    //            if (item.sum > weekLimit)
    //            {
    //                item.sum = weekLimit;
    //            }
    //        }
    //        return weekWiseRates;
    //    }

    //    private static List<WeekWiseRates> _WeekWiseRates(List<DayWiseRates> dayWiseRates)
    //    {
    //        List<WeekWiseRates> weekWiseRates = new List<WeekWiseRates>();
    //        for (int i = 0; i < dayWiseRates.Count; i++)
    //        {
    //            decimal number = i / 7;
    //            int weekIndex = Convert.ToInt32(number);
    //            if (weekWiseRates.Exists(x => x.weekIndex == weekIndex))
    //            {
    //                WeekWiseRates item = weekWiseRates.FirstOrDefault(x => x.weekIndex == weekIndex);
    //                item.sum += dayWiseRates[i].sum;
    //            }
    //            else
    //            {
    //                weekWiseRates.Add(new WeekWiseRates() { weekIndex = weekIndex, sum = dayWiseRates[i].sum });
    //            }
    //        }
    //        return weekWiseRates;
    //    }

    //    private static List<DayWiseRates> _ApplyDayWiseLimit(List<DayWiseRates> dayWiseRates, List<ChargeSheetPrices> ChargeSheetPrices)
    //    {
    //        decimal dayLimit = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.MAXIMUM_PRICE_DAILY_PRICE.ToString())).AttributeValue);
    //        foreach (DayWiseRates item in dayWiseRates)
    //        {
    //            if (item.sum > dayLimit)
    //            {
    //                item.sum = dayLimit;
    //            }
    //        }
    //        return dayWiseRates;
    //    }

    //    private static List<DayWiseRates> _DayWiseRates(List<CustomHours> hours)
    //    {
    //        List<DayWiseRates> dayWiseRates = new List<DayWiseRates>();
    //        foreach (CustomHours item in hours)
    //        {
    //            DateTime date = Convert.ToDateTime(_RemoveTimeStamp(item.startHour));
    //            if (dayWiseRates.Exists(x => _GetTime(x.date) == _GetTime(date)))
    //            {
    //                DayWiseRates rateSum = dayWiseRates.FirstOrDefault(x => _GetTime(x.date) == _GetTime(date));
    //                rateSum.sum += item.rate;
    //            }
    //            else
    //            {
    //                dayWiseRates.Add(new DayWiseRates() { date = Convert.ToDateTime(_RemoveTimeStamp(item.startHour)), sum = item.rate });
    //            }
    //        }
    //        return dayWiseRates;
    //    }

    //    private static long _GetTime(DateTime dateTime)
    //    {
    //        long retval = 0;
    //        DateTime standardTime = new DateTime(1970, 1, 1);
    //        TimeSpan timeSpan = (dateTime - standardTime);
    //        retval = (long)(timeSpan.TotalMilliseconds + 0.5);
    //        return retval;
    //    }

    //    private static decimal _FindRateForHour(DateTime startHour, DateTime endHour, List<ChargeSheetPrices> ChargeSheetPrices, int hourNumber)
    //    {
    //        List<ExceptionDates> exceptionDates = JsonConvert.DeserializeObject<List<ExceptionDates>>(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.EXCEPTION_DATES.ToString())).AttributeValue);
    //        List<DurationBaseRates> hourBasedRates = JsonConvert.DeserializeObject<List<DurationBaseRates>>(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.HOURS_BASED_RATES.ToString())).AttributeValue);
    //        List<HourBasedRates> durationBasedRates = JsonConvert.DeserializeObject<List<HourBasedRates>>(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.DURATION_BASED_RATES.ToString())).AttributeValue);
    //        decimal basePrice = Convert.ToDecimal(ChargeSheetPrices.FirstOrDefault(x => x.AttributeKey.Equals(ChargeSheetPriceRules.BASE_PRICE.ToString())).AttributeValue);

    //        var rate = exceptionDates.FirstOrDefault(x => _MatchDateOnly(Convert.ToDateTime(x.date), startHour))?.ratePerHour;

    //        if (rate == null)
    //        {
    //            foreach (DurationBaseRates item in hourBasedRates)
    //            {
    //                rate = item.hourPrices.FirstOrDefault(x => _MatchDayHour(Convert.ToDateTime(x.fromTime), Convert.ToDateTime(x.toTime), startHour, endHour, item.dayOfWeek))?.ratePerHour;
    //                if (rate == null)
    //                    continue;
    //                else
    //                    break;
    //            }
    //        }

    //        if (rate == null)
    //        {
    //            if (hourNumber == 0)
    //            {
    //                rate = basePrice;
    //            }
    //            else
    //            {
    //                if (hourNumber > durationBasedRates.Count)
    //                    rate = durationBasedRates.LastOrDefault().baseRate;
    //                else
    //                    rate = durationBasedRates[hourNumber - 1] != null ? durationBasedRates[hourNumber - 1].baseRate : durationBasedRates.LastOrDefault().baseRate;
    //            }
    //        }

    //        return rate == null ? 0 : Convert.ToDecimal(rate);
    //    }

    //    private static string _RemoveTimeStamp(DateTime dateTime)
    //    {
    //        return dateTime.ToShortDateString();
    //    }

    //    private static bool _MatchDateOnly(DateTime date, DateTime startHour)
    //    {
    //        var IsMatched = (date.Year == startHour.Year && date.Month == startHour.Month && date.Day == startHour.Day);
    //        return IsMatched;
    //    }

    //    private static bool _MatchDayHour(DateTime fromTime, DateTime toTime, DateTime startHour, DateTime endHour, string dayOfWeek)
    //    {
    //        bool IsMatched = dayOfWeek == startHour.ToString(Constants.DayOfWeekFormat) && dayOfWeek == endHour.ToString(Constants.DayOfWeekFormat) &&
    //            _GetTwelveHours(fromTime) <= _GetTwelveHours(startHour) && _GetTwelveHours(toTime) >= _GetTwelveHours(endHour);
    //        return IsMatched;
    //    }

    //    private static DateTime _RemoveSeconds(DateTime dateTime)
    //    {
    //        return dateTime.AddSeconds(-dateTime.Second);
    //    }

    //    private static DateTime _AddMinutes(DateTime dateTime, int minutes)
    //    {
    //        return dateTime.AddMinutes(minutes);
    //    }

    //    private static int _GetTwelveHours(DateTime datetime)
    //    {
    //        int result = datetime.Hour % 12 == 0 ? 12 : datetime.Hour % 12;
    //        return result;
    //    }
    //    #endregion
    //}
}