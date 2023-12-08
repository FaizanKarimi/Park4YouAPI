using System;
using System.IO;
using Components.Services.Interfaces;
using Infrastructure.Enums;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace Components.Services.Implementation
{
    public class PdfService : IPdfService
    {
        #region Constructor
        public PdfService()
        {

        }
        #endregion

        #region Public Methods
        public bool ConvertParkingReport(string html, string fileName)
        {
            bool IsSaved = false;
            Document document = new Document();
            try
            {
                string newFilePath = string.Concat(CommonHelpers.GetDomainName(), @"\ParkingReports\", fileName);
                using (FileStream fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    PdfWriter.GetInstance(document, fileStream);
                    document.Open();
                    HtmlWorker htmlWorker = new HtmlWorker(document);
                    htmlWorker.Parse(new StringReader(html));
                    document.Close();
                    htmlWorker.Close();
                }
                IsSaved = true;
            }
            catch (Exception)
            {
                throw new Park4YouException(ErrorMessages.UNABLE_TO_CONVERT_TO_PDF);
            }
            return IsSaved;
        }

        public bool ConvertProfileReport(string html, string fileName)
        {
            bool IsSaved = false;
            Document document = new Document();
            try
            {
                string newFilePath = string.Concat(CommonHelpers.GetDomainName(), @"\ProfileReports\", fileName);
                using (FileStream fileStream = new FileStream(newFilePath, FileMode.Create))
                {
                    PdfWriter.GetInstance(document, fileStream);
                    document.Open();
                    StyleSheet styles = new StyleSheet();
                    HtmlWorker htmlWorker = new HtmlWorker(document);
                    htmlWorker.Parse(new StringReader(html));
                    document.Close();
                }
                IsSaved = true;
            }
            catch (Exception)
            {
                throw new Park4YouException(ErrorMessages.UNABLE_TO_CONVERT_TO_PDF);
            }
            return IsSaved;
        }
        #endregion
    }
}