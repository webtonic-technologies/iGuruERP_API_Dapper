namespace Infirmary_API.DTOs.Responses
{
    public class GetStockEntriesExportResponse
    {
        public string ItemTypeName { get; set; }
        public string MedicineName { get; set; }
        public string Company { get; set; }
        public string BatchCode { get; set; }
        public string Diagnosis { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerQuantity { get; set; }
        public string ExpiryDate { get; set; }  // Format: "DD-MM-YYYY"
        public string EntryDate { get; set; }   // Format: "DD-MM-YYYY"
        public string DosageDetails { get; set; }
    }
}
