namespace HostelManagement_API.DTOs.Requests
{
    public class GetStudentRequest
    {
        public int InstituteID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }
}
