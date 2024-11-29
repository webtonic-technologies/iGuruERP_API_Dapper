namespace Transport_API.DTOs.Requests
{
    public class RoutePlanRequestDTO
    {
        public int RoutePlanID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int VehicleID { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<RouteStop>? RouteStops { get; set; }
    }

    public class RouteStop
    {
        public int StopID { get; set; }
        public int RoutePlanID { get; set; }
        public string StopName { get; set; } = string.Empty;
        public string PickUpTime { get; set; } = string.Empty;
        public string DropTime { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }

        // New properties to handle different types of payments
        public List<SinglePaymentDTO>? SinglePayment { get; set; }
        public List<TermPaymentDTO>? TermPayment { get; set; }
        public List<MonthlyPaymentDTO>? MonthlyPayment { get; set; }
    }

    public class SinglePaymentDTO
    {
        public int RSFPID { get; set; }
        public int RoutePlanID { get; set; }
        public int StopID { get; set; }
        public decimal FeesAmount { get; set; }
    }

    public class TermPaymentDTO
    {
        public int RTFPID { get; set; }
        public int RoutePlanID { get; set; }
        public int StopID { get; set; }
        public string TermName { get; set; } = string.Empty;
        public decimal FeesAmount { get; set; }
        public string DueDate { get; set; }
    }

    public class MonthlyPaymentDTO
    {
        public int RMFPID { get; set; }
        public int RoutePlanID { get; set; }
        public int StopID { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal FeesAmount { get; set; }
    }
}
