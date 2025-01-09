namespace Transport_API.DTOs.Responses
{
    public class GetFuelExpenseExportResponse
    {
        public int VehicleFuelExpenseID { get; set; }
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }  // Added VehicleNumber

        public decimal CurrentReadingInKM { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal DistanceTravelledInKM { get; set; }
        public decimal PreviousFuelAddedInLitre { get; set; }
        public decimal FuelAddedInLitre { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string ExpenseDate { get; set; }  // Format: DD-MM-YYYY
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
    }
}
