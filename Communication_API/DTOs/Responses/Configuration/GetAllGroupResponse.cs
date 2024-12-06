namespace Communication_API.DTOs.Responses.Configuration
{
    public class GetAllGroupResponse
    {
        public int GroupID { get; set; }
        public string AcademicYearCode { get; set; }
        public string GroupName { get; set; }
        public string UserType { get; set; }  // 'Student' or 'Employee'
        public int Count { get; set; }        // Number of students/employees
    }
}
