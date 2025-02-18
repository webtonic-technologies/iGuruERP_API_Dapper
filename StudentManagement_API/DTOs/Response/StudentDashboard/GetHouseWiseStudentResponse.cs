using System.Collections.Generic;

namespace StudentManagement_API.DTOs.Responses
{
    public class GetHouseWiseStudentResponse
    {
        public List<HouseWiseStudent> StudentHouses { get; set; }
    }

    public class HouseWiseStudent
    {
        public string HouseType { get; set; }
        public int StudentCount { get; set; }
    }
}
