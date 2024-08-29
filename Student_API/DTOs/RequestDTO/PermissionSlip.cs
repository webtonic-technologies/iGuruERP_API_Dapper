using System.Text.Json.Serialization;

namespace Student_API.DTOs.RequestDTO
{
    public class PermissionSlip
    {
        public int Student_Id { get; set; }
        public int Student_Parent_Info_id { get; set; }
        public int Institute_id { get; set; }
        public string Reason { get; set; }
        [JsonIgnore]
        public string Qr_Code { get; set; }
    }
}
