namespace Transport_API.DTOs.Response
{
    public class TransportSurveillance
    {
        public int SurveillanceId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string VideoFileLink { get; set; } = string.Empty;
        public int InstituteId { get; set; }
    }
}
