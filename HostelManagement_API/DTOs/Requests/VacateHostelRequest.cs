namespace HostelManagement_API.DTOs.Requests
{
    public class VacateHostelRequest
    {
        public int StudentID { get; set; }
        public int InstituteID { get; set; }
        public string VacateDate { get; set; }
    }
}
