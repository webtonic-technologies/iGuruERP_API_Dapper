using System.Collections.Generic;

namespace TimeTable_API.DTOs.Responses
{
    public class EmployeeWiseResponse
    {
        public string Day { get; set; }
        public List<EmployeeWiseSessionResponse> Sessions { get; set; }

        public EmployeeWiseResponse()
        {
            Sessions = new List<EmployeeWiseSessionResponse>();
        }
    }

    public class EmployeeWiseSessionResponse
    {
        public string Day { get; set; }
        public string SessionName { get; set; }
        public string ClassNameSection { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
