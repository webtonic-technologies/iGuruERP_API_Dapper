using System.Collections.Generic;

namespace Communication_API.DTOs.Responses.NoticeBoard
{
    public class EmployeeCircularStatisticsResponse
    {
        public int TotalEmployee { get; set; }
        public int Viewed { get; set; }
        public int NotViewed { get; set; }
        public List<EmployeeDetail> EmployeeList { get; set; }

        public class EmployeeDetail
        {
            public string EmployeeName { get; set; }
            public string DepartmentDesignation { get; set; }
            public string EmployeeCode { get; set; }
            public string? ViewedOn { get; set; }
        }
    }
}
