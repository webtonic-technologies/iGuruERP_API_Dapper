namespace TimeTable_API.DTOs.Requests
{
    public class EmployeeWiseRequest
    {
        public string AcademicYearCode { get; set; }
        public int EmployeeID { get; set; }
        public int InstituteID { get; set; }
    }
}
