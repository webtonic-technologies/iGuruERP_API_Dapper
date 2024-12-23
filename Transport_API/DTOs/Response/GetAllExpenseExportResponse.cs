namespace Transport_API.DTOs.Responses
{
    public class GetAllExpenseExportResponse
    {
        public string VehicleNumber { get; set; }
        public string ExpenseType { get; set; }
        public string ExpenseDate { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
    }
}
