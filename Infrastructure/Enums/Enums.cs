namespace Infrastructure.Enums
{
    public enum ChargeSheetPriceRules
    {
        BASE_PRICE,
        HOURS_BASED_RATES,
        DURATION_BASED_RATES,
        EXCEPTION_DATES,
        MAXIMUM_PRICE_DAILY_PRICE,
        MAXIMUM_PRICE_WEEKLY_PRICE,
        MAXIMUM_PRICE_MONTHLY_PRICE
    }

    public enum ChargeSheetRule
    {
        NONE = 0,
        BASE_PRICE = 1,
        HOURLY_CHANGE = 2,
        DURATION_CHANGE = 3,
        EXCEPTIONS = 4,
        MAXIMUM_PRICES = 5
    }

    public enum UserRoles
    {
        Admin,
        User,
        Vendor
    }

    public enum ParkingStatus
    {
        STOP,
        START
    }

    public enum UserSetting
    {
        CLOCK_TYPE,
        SOUND,
        RECENT_CAR,
        RECENT_AREA,
        PARKING_FIXED_NOTIFICATION_FIVETEEN_MINUTES,
        PARKING_FIXED_NOTIFICATION_THIRTY_MINUTES,
        PARKING_PDF_RECEIPT,
        LANGUAGE
    }

    public enum Flag
    {
        TRUE,
        FALSE
    }

    public enum RequestStatus
    {
        FAILED,
        SUCCESS
    }

    public enum ParkingPushNotificationSettings
    {
        PARKING_FIXED_NOTIFICATION_FIVETEEN_MINUTES,
        PARKING_FIXED_NOTIFICATION_THIRTY_MINUTES
    }

    public enum Vendors
    {
        ADMIN,
        SOLVISION,
        PRS
    }
}