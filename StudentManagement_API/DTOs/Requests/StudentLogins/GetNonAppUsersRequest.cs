namespace StudentManagement_API.DTOs.Requests
{
    public class GetNonAppUsersRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string Search { get; set; }
    }
}
