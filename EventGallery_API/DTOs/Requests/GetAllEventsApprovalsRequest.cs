namespace EventGallery_API.DTOs.Requests
{
    public class GetAllEventsApprovalsRequest
    {
        public int InstituteID { get; set; } // InstituteID column from tblEvent
        public string Search { get; set; } // Search by EventName
        public string AcademicYearCode { get; set; } // New field for AcademicYearCode

    }
}
