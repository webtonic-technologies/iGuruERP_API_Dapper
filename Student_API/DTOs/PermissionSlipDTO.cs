namespace Student_API.DTOs
{
    public class PermissionSlipDTO
    {
        public int PermissionSlipId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string GenderName { get; set; }
        public string ParentName { get; set; }
        public DateTime RequestedDateTime { get; set; }
        public string Reason { get; set; }
        public bool IsApproved { get; set; }
    }
}
