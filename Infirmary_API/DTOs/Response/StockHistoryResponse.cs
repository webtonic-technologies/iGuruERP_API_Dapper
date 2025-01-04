namespace Infirmary_API.DTOs.Responses
{
    public class StockHistoryResponse
    {
        public string QtyID { get; set; }  // Should include the "SA#" prefix
        public string BatchCode { get; set; }
        public int QuantityAdjusted { get; set; }
        public int CurrentStock { get; set; }
        public string UpdatedBy { get; set; }  // Format: Employee Name on 14-02-2024 at 01:00 PM
        public string Reason { get; set; }
    }
}
