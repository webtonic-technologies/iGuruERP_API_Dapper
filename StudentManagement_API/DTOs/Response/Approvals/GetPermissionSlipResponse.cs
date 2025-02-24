namespace StudentManagement_API.DTOs.Responses
{
    public class GetPermissionSlipResponse
    {
        public int PermissionSlipID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; } // New: Admission Number from tbl_StudentMaster
        public string Class { get; set; }
        public string Section { get; set; }
        public string Gender { get; set; }
        // This will display the parent's type (e.g., Father, Mother, Guardian)
        public string RequestedBy { get; set; }
        // Parent name from tbl_StudentParentsInfo (if available)
        public string ParentName { get; set; }
        // Formatted as "dd-MM-yyyy at hh:mm tt"
        public string RequestedDateTime { get; set; }
        public string Reason { get; set; }
        public string PickedUp { get; set; } 

    }
}
