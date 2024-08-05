namespace Transport_API.Models
{
    public class RoutePlan
    {
        public int RoutePlanID { get; set; }
        public string RouteName { get; set; }
        public int VehicleID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }

    }
}
