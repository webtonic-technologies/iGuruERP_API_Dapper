namespace InfirmaryVisit_API.Models
{
    public class StudentMaster
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public string AdmissionNumber { get; set; }
        public string RollNumber { get; set; }
    }
}
