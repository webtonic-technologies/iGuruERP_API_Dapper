using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Vehicle
{
    //public int VehicleId { get; set; }
    //public string VehicleNumber { get; set; }
    //public string VehicleModel { get; set; }
    //public int EmployeeId { get; set; }
    //public int InstituteId { get; set; }
    //public DateTime RenewalDate { get; set; }
    //public int SeatingCapacity { get; set; }
    //public string ChassisNumber { get; set; }
    //public string GPSIMEINumber { get; set; }



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
    public bool IsActive { get; set; }

}
