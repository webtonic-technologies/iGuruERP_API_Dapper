namespace Transport_API.DTOs.Responses
{
    public class GetFuelExpenseResponse
    {
        public int VehicleFuelExpenseID { get; set; }
        public int VehicleID { get; set; }
        public decimal CurrentReadingInKM { get; set; }
        public decimal PreviousReading { get; set; }
        public decimal DistanceTravelledInKM { get; set; }
        public decimal PreviousFuelAddedInLitre { get; set; }
        public decimal FuelAddedInLitre { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string ExpenseDate { get; set; }
        public string Remarks { get; set; }
        public int InstituteID { get; set; }
        public List<string> Documents { get; set; }  // Holds paths of the documents
    }
}
