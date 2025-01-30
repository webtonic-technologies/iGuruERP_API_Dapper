//namespace HostelManagement_API.DTOs.Responses
//{
//    public class GetAllFloorsResponse
//    {
//        public string BuildingName { get; set; }
//        public List<string> Floors { get; set; }

//        public GetAllFloorsResponse()
//        {
//            Floors = new List<string>();
//        }
//    }
//}




namespace HostelManagement_API.DTOs.Responses
{
    public class GetAllFloorsResponse
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public List<FloorResponse1> Floors { get; set; }

        public GetAllFloorsResponse()
        {
            Floors = new List<FloorResponse1>();
        }
    }

    public class FloorResponse1
    {
        public int FloorID { get; set; }
        public string FloorName { get; set; }
    }
}
