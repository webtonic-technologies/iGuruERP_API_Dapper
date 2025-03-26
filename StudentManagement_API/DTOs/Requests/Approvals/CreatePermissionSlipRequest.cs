namespace StudentManagement_API.DTOs.Requests
{
    public class CreatePermissionSlipRequest
    {
        public int InstituteID { get; set; }
        public int StudentID { get; set; }
        public int ParentID { get; set; }
        public int RequestedBy { get; set; }
        public string PickedUpBy { get; set; }
        public string Reason { get; set; }
        public string AcademicYearCode { get; set; }
    }
}
