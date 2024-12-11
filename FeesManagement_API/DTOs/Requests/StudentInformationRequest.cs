namespace FeesManagement_API.DTOs.Requests
{
    public class StudentInformationRequest
    { 
        public int InstituteID { get; set; }
        public int? StudentID { get; set; }
        public string? Search { get; set; }
    }
}
