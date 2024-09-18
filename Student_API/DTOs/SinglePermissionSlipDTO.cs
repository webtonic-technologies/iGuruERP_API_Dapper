namespace Student_API.DTOs
{
    public class SinglePermissionSlipDTO
    {
        public int PermissionSlip_Id { get; set; }
        public int Student_Id { get; set; }
        public string StudentName { get; set; }
        public string Admission_Number { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string GenderName { get; set; }
        public string ParentName { get; set; }
        public string RequestedDateTime { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }
        public string Qr_Code { get; set; }
        public string Parent_File { get; set; }
    }
}
