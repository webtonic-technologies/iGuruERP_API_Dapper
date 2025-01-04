namespace Infirmary_API.DTOs.Responses
{
    public class GetStockHistoryExportResponse
    {
        public string QtyID { get; set; }        // Quantity ID (with prefix SA#)
        public string BatchCode { get; set; }    // Batch code of the stock item
        public int QuantityAdjusted { get; set; } // The adjusted quantity
        public int CurrentStock { get; set; }    // Current stock
        public string UpdatedBy { get; set; }    // The person who updated the stock (Employee Name and timestamp)
        public string Reason { get; set; }       // The reason for stock change
    }
}
