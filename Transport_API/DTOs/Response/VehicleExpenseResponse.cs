namespace Transport_API.DTOs.Response
{
    public class VehicleExpenseResponse
    {
        public int VehicleExpenseId { get; set; }
        public int ExpenseTypeId { get; set; }
        public DateTime Date { get; set; }
        public decimal Cost { get; set; }
        public string Remarks { get; set; }
    }
}
