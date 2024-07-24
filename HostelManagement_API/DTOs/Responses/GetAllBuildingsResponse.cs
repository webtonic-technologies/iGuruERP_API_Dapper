namespace HostelManagement_API.DTOs.Responses
{
    public class GetAllBuildingsResponse
    {
        public string BlockName { get; set; }
        public List<string> Buildings { get; set; }

        public GetAllBuildingsResponse()
        {
            Buildings = new List<string>();
        }
    }
}
