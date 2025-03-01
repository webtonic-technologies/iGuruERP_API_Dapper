namespace StudentManagement_API.DTOs.Responses
{
    public class GetNonAppUsersExportResponse
    {
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        public string PrimaryMobileNo { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
    }
}
