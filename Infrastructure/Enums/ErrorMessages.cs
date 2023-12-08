namespace Infrastructure.Enums
{
    public enum ErrorMessages
    {
        #region Application
        EXCEPTION_MESSAGE,
        ERROR_OCCURED_ON_UPDATING,
        UNABLE_TO_CONVERT_TO_PDF,        
        INTERNAL_SERVER_ERROR,
        ROLE_ALREADY_EXIST,
        #endregion

        #region Parking        
        PARKING_NOT_EXIST,
        PARKING_LOT_NOT_EXIST,
        PARKING_ALREADY_ENDED,
        #endregion

        #region ChargeSheet
        CHARGE_SHEET_DOES_NOT_EXIST,
        CHARGE_SHEET_PRICES_DOES_NOT_EXIST,
        #endregion                

        #region User
        USER_AREA_DOES_NOT_EXIST,
        USER_CARD_NOT_EXIST,
        USER_CARD_EXPIRED,
        USER_PROFILE_NOT_EXIST,
        USER_PROFILE_ALREADY_EXIST,
        USER_CARD_INVALID,
        USER_EMAIL_NOT_FOUND,
        USER_EMAIL_ALREADY_EXIST,
        USER_NO_LATEST_AREA_EXIST,
        USER_INVALID_VERIFICATION_CODE,
        USER_NOT_EXIST,
        USER_INVALID_USERNAME_PASSWORD,
        USER_ALREADY_EXIST,
        USER_MOBILE_NUMBER_ALREADY_EXIST,
        USER_NOT_CREATED,
        USER_NOT_ADDED_TO_ROLE,
        USER_INVALID_CURRENT_PASSWORD,
        USER_NOT_SUCCESSFULLY_REGISTERED,
        USER_SETTING_NOT_EXIST,
        UNABLE_TO_DELETE_USER,
        DEVICE_NOT_FOUND,
        USER_CARD_CANNOT_BE_DELETED_BECAUSE_USED_IN_PARKING,
        USER_SETTING_INCORRECT_ATTRIBUTE_KEY,
        #endregion

        #region Vehicle
        VEHICLE_DOES_NOT_EXIST,
        VEHICLE_CANNOT_BE_DELETED_BECAUSE_USED_IN_PARKING,
        VEHICLE_ALREADY_EXIST,
        #endregion

        #region Services
        ERROR_OCCURED_WHILE_SENDING_MESSAGE_USING_TWILLIO,
        #endregion

        #region Solvision
        SOLVISION_PARKING_REQUEST_FAILED,
        SOLVISION_PARKING_REQUEST_LOGIN_FAILED,
        SOLVISION_PARKING_LOGIN_ERROR,
        SOLVISION_PARKING_REQUEST_ERROR,
        #endregion

        #region PRS(KK)
        PRS_PARKING_REQUEST_FAILED,
        PRS_PARKING_UPDATE_REQUEST_FAILED
        #endregion
    }
}