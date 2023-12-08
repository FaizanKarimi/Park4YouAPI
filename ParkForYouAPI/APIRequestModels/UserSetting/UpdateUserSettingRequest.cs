using System.Collections.Generic;

namespace ParkForYouAPI.APIRequestModels.UserSetting
{
    public class UpdateUserSettingRequest
    {
        public int Id { get; set; }
        public string AttributeKey { get; set; }
        public string AttributeValue { get; set; }
    }
}