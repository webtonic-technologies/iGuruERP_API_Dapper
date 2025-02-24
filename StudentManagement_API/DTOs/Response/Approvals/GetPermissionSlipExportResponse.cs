namespace StudentManagement_API.DTOs.Responses
{
    public class GetPermissionSlipExportResponse
    {
        public string StudentName { get; set; }
        public string AdmissionNo { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string Gender { get; set; }
        public string RequestedBy { get; set; }
        public string ParentName { get; set; }
        public string RequestedDateTime { get; set; }
        public string Reason { get; set; }
        public string PickedUp { get; set; }
    }
}
