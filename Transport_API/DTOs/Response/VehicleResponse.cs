namespace Transport_API.DTOs.Response
{
    public class VehicleResponse
    {

        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public string RenewalYear { get; set; }
        public int VehicleTypeID { get; set; }
        public string VehicleTypeName { get; set; }
        public int FuelTypeID { get; set; }
        public string FuelTypeName { get; set; }
        public int SeatingCapacity { get; set; }
        public string ChassieNo { get; set; }
        public string InsurancePolicyNo { get; set; }
        public DateTime RenewalDate { get; set; }
        public int AssignDriverID { get; set; }
        public string AssignDriverName { get; set; }
        public string GPSIMEINo { get; set; }
        public string TrackingID { get; set; }
        public bool IsActive { get; set; }
    }
}
