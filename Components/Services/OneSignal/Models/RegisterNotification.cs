using System.Collections.Generic;

namespace Components.Services.OneSignal.Models
{
    public class RegisterNotification
    {
        public string id { get; set; }
        public int recipients { get; set; }
        public List<string> errors { get; set; }
    }
}