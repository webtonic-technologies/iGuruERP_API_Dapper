namespace Communication_API.DTOs.Responses.Configuration
{
    public class GetGroupMembersResponse
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int TotalCount { get; set; }   // Total students or employees in the group
        public List<MemberDetails> Members { get; set; }
    }

    public class MemberDetails
    {
        public string Name { get; set; }  // For students and employees
        public string ClassSection { get; set; } // Class-Section for students
        public string DepartmentDesignation { get; set; } // Department-Designation for employees
        public string MobileNumber { get; set; }
    }
}
