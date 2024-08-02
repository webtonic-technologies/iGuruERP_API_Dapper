namespace Transport_API.DTOs.Requests
{
    public class GetAllVehiclesRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteID { get; set; }
        public string SearchText { get; set; }
    }

    public class VehicleRequest
    {

        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }
        public string VehicleModel { get; set; }
        public string RenewalYear { get; set; }
        public int VehicleTypeID { get; set; }
        public int FuelTypeID { get; set; }
        public int SeatingCapacity { get; set; }
        public string ChassieNo { get; set; }
        public string InsurancePolicyNo { get; set; }
        public DateTime RenewalDate { get; set; }
        public int AssignDriverID { get; set; }
        public string GPSIMEINo { get; set; }
        public string TrackingID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }

        public List<VehicleDocumentRequest> VehicleDocuments { get; set; }

    }

    public class VehicleDocumentRequest
    {
        public int VehicleDocumentID { get; set; }
        public string VehicleDocument { get; set; }
        public int VehicleID { get; set; }
    }
}
