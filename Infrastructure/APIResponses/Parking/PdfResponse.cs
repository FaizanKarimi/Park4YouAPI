namespace Infrastructure.APIResponses.Parking
{
    public class PdfResponse
    {
        public PdfResponse()
        {
            this.Success = true;
            this.ErrorMessasge = string.Empty;
        }

        public dynamic Data { get; set; }
        public bool Success { get; set; }
        public string ErrorMessasge { get; set; }
    }
}
