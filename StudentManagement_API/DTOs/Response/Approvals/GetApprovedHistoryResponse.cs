namespace StudentManagement_API.DTOs.Responses
{
    public class GetApprovedHistoryResponse
    {
        public int PermissionSlipID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; } // New: Admission Number from tbl_StudentMaster
        public string Class { get; set; }
        public string Section { get; set; }
        // ApprovalDate is formatted as "DD-MM-YYYY"
        public string ApprovalDate { get; set; }
        public string RequestedBy { get; set; }
        public string ParentName { get; set; }
        public string Reason { get; set; }
        public string PickedUpBy { get; set; }
        public string ApprovedBy { get; set; }
    }
}
