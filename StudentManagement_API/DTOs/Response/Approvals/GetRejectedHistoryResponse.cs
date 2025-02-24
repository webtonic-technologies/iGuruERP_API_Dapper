namespace StudentManagement_API.DTOs.Responses
{
    public class GetRejectedHistoryResponse
    {
        public int PermissionSlipID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; } // New: Admission Number from tbl_StudentMaster
        public string Class { get; set; }
        public string Section { get; set; }
        // RejectionDate is formatted as "dd-MM-yyyy" (using ModifiedDate)
        public string RejectionDate { get; set; }
        public string RequestedBy { get; set; }
        public string ParentName { get; set; }
        public string Reason { get; set; }
        public string RejectedBy { get; set; }
    }
}
