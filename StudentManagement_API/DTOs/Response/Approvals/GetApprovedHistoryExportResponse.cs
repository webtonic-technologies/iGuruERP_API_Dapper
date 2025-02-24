namespace StudentManagement_API.DTOs.Responses
{
    public class GetApprovedHistoryExportResponse
    {
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        // ApprovalDate is formatted as "dd-MM-yyyy"
        public string ApprovalDate { get; set; }
        public string RequestedBy { get; set; }
        public string ParentName { get; set; }
        public string Reason { get; set; }
        public string PickedUp { get; set; }
        public string ApprovedBy { get; set; }
    }
}
