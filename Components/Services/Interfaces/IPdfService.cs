namespace Components.Services.Interfaces
{
    public interface IPdfService
    {
        bool ConvertParkingReport(string html, string fileName);

        bool ConvertProfileReport(string html, string fileName);
    }
}