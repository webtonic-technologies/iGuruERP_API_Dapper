namespace StudentManagement_API.DTOs.Responses
{
    public class GetRejectedHistoryExportResponse
    {
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        // RejectionDate formatted as "dd-MM-yyyy"
        public string RejectionDate { get; set; }
        public string RequestedBy { get; set; }
        public string ParentName { get; set; }
        public string Reason { get; set; }
        public string RejectedBy { get; set; }
    }
}
