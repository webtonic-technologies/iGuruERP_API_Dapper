namespace StudentManagement_API.DTOs.Responses
{
    public class GetNonAppUsersResponse
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public string Class { get; set; }
        public string Section { get; set; }
        // Mobile number from tbl_StudentParentsInfo where Parent_Type_id = 1
        public string PrimaryMobileNo { get; set; }
        // Login credentials from tblLoginInformationMaster
        public string LoginID { get; set; }
        public string Password { get; set; }
    }
}
