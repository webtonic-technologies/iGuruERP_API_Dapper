using System.Collections.Generic;

namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class StudentCircularStatisticsResponse
    {
        public int TotalStudent { get; set; }
        public int Viewed { get; set; }
        public int NotViewed { get; set; }
        public List<StudentDetails> StudentList { get; set; }

        public class StudentDetails
        {
            public string StudentName { get; set; }
            public string ClassSection { get; set; }
            public string AdmissionNumber { get; set; }
            public string? ViewedOn { get; set; }
        }
    }
}
