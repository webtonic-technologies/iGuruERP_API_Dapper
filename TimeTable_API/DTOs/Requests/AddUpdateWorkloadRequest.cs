namespace TimeTable_API.DTOs.Requests
{
    public class AddUpdateWorkloadRequest
    {
        public int? WorkLoadID { get; set; } // If null, create a new workload entry
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectID { get; set; }
        public int EmployeeID { get; set; }
        public decimal WorkLoad { get; set; }
    }
}
