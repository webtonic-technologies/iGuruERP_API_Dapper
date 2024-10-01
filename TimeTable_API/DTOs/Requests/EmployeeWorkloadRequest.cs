namespace TimeTable_API.DTOs.Requests
{
    public class EmployeeWorkloadRequest
    {
        public string AcademicYearCode { get; set; }
        public int EmployeeID { get; set; }
        public int InstituteID { get; set; }
    }
}
