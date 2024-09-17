namespace Employee_API.DTOs
{
    public class EmployeeGenderStatsResponse
    {
        public int Male { get; set; }
        public string MalePercentage { get; set; } = string.Empty;
        public int Female { get; set; }
        public string FemalePercentage { get; set; } = string.Empty;
    }
    public class EmployeeGenderStats
    {
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public double MalePercentage { get; set; }
        public double FemalePercentage { get; set; }
    }
    public class AgeGroupStats
    {
        public int Age_18_25 { get; set; }
        public int Age_26_30 { get; set; }
        public int Age_31_35 { get; set; }
        public int Age_36_40 { get; set; }
        public int Age_41_45 { get; set; }
        public int Age_46_50 { get; set; }
        public int Age_51_55 { get; set; }
        public int Age_56_60 { get; set; }
        public int Age_60_Plus { get; set; }

    }
    public class AgeGroupStatsResponse
    {
        public string AgeGroup { get; set; } = string.Empty;  // The age group range
        public int AgeCount { get; set; }     // The count of employees in this age group
    }
    public class ExperienceStats
    {
        public int Experience_0_1 { get; set; }
        public int Experience_1_2 { get; set; }
        public int Experience_2_3 { get; set; }
        public int Experience_3_4 { get; set; }
        public int Experience_4_5 { get; set; }
        public int Experience_5_6 { get; set; }
        public int Experience_6_7 { get; set; }
        public int MaxExperience { get; set; }
        public int MinExperience { get; set; }
        public double AvgExperience { get; set; }
    }
    public class ExperienceRangeResponse
    {
        public string Experience { get; set; }  // Experience range in the format "0-1", "1-2", etc.
        public int Count { get; set; }          // The count of employees in that experience range
    }
    public class DepartmentEmployeeCount
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }
    public class DepartmentEmployeeResponse
    {
        public List<DepartmentEmployeeCount> Departments { get; set; } = new();
        public int TotalEmployees { get; set; }
    }
    public class EmployeeEvent
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
    }

    public class EmployeeEventsResponse
    {
        public List<EmployeeEvent> Birthdays { get; set; } = new();
        public List<EmployeeEvent> WorkAnniversaries { get; set; } = new();
    }
    public class GenderCountByDesignation
    {
        public int DesignationId { get; set; }
        public string Designation { get; set; } = string.Empty;
        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
    }
    public class EmployeeStatusData
    {
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int TotalCount { get; set; }
    }

    public class ActiveInactiveEmployeesResponse
    {
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public double ActivePercentage { get; set; }
        public double InactivePercentage { get; set; }
    }
    public class AppUserNonAppUserResponse
    {
        public int AppUsers { get; set; }
        public int NonAppUsers { get; set; }
    }
}
