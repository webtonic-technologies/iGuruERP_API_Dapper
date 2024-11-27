namespace EventGallery_API.DTOs.Requests.Approvals
{
    public class GetAllHolidaysApprovalsRequest
    {
        public int InstituteID { get; set; }
        public string Search { get; set; }
        public string AcademicYearCode { get; set; }
    }
}
