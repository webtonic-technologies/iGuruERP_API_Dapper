namespace Infirmary_API.DTOs.Requests
{
    public class EnterInfirmaryStockAdjustmentRequest
    {
        public int StockID { get; set; }
        public int Quantity { get; set; }
        public string BatchCode { get; set; }
        public string Reason { get; set; }
        public string EntryDate { get; set; }  // Format 'DD-MM-YYYY'
        public int StockManagerID { get; set; }
    }
}
