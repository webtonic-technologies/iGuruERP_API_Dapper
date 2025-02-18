using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Responses
{
    public class GetStudentTypeStatisticsResponse
    {
        // Note: Although the prompt shows a singular "StudentType", we return a list so that each type (e.g. Hosteller, Day Schoolar) is included.
        public List<StudentTypeStatistic> StudentType { get; set; }
    }

    public class StudentTypeStatistic
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
