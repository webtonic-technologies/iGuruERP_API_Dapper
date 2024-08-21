namespace HostelManagement_API.DTOs.Responses
{
    public class GetAllFloorsResponse
    {
        public string BuildingName { get; set; }
        public List<string> Floors { get; set; }

        public GetAllFloorsResponse()
        {
            Floors = new List<string>();
        }
    }
}
