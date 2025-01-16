namespace Communication_API.DTOs.Responses
{
    public class GetAllDiaryExportResponse
    {
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassSection { get; set; }
        public string Subject { get; set; }
        public string DiaryRemarks { get; set; }
        public string ShareOn { get; set; }
        public string GivenBy { get; set; }
    }
}
