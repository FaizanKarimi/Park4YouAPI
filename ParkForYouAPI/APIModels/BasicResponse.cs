namespace ParkForYouAPI.APIModels
{
    public class BasicResponse
    {
        public BasicResponse()
        {
            this.Success = true;
            this.ErrorMessasge = string.Empty;
        }

        public dynamic Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessasge { get; set; }        
    }
}