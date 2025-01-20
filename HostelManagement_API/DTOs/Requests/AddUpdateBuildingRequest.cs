//namespace HostelManagement_API.DTOs.Requests
//{
//    public class AddUpdateBuildingRequest
//    {
//        public int? BuildingID { get; set; }
//        public string BuildingName { get; set; }
//        public int BlockID { get; set; }
//        public int InstituteID { get; set; }
//        public bool IsActive { get; set; }
//    }

//    public class AddUpdateBuildingsRequest
//    {
//        public List<AddUpdateBuildingRequest> Buildings { get; set; }
//    }
//}



namespace HostelManagement_API.DTOs.Requests
{
    public class AddUpdateBuildingRequest
    {
        public int BlockID { get; set; }  // BlockID should be directly inside the building object
        public int BuildingID { get; set; }
        public string BuildingName { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }

    public class AddUpdateBuildingsRequest
    {
        public List<AddUpdateBuildingRequest> Buildings { get; set; }
    }
}
