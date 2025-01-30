//namespace HostelManagement_API.DTOs.Responses
//{
//    public class GetAllBuildingsResponse
//    {
//        public string BlockName { get; set; }
//        public List<string> Buildings { get; set; }

//        public GetAllBuildingsResponse()
//        {
//            Buildings = new List<string>();
//        }
//    }
//}



namespace HostelManagement_API.DTOs.Responses
{
    public class GetAllBuildingsResponse
    {
        public int BlockID { get; set; }
        public string BlockName { get; set; }

        public List<BuildingResponse1> Buildings { get; set; }

        public GetAllBuildingsResponse()
        {
            Buildings = new List<BuildingResponse1>();
        }
    }

    public class BuildingResponse1
    {
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
    }
}
