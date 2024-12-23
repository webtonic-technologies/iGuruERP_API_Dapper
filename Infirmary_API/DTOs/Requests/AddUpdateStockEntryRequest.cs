namespace Infirmary_API.DTOs.Requests
{
    public class AddUpdateStockEntryRequest
    {
        public int StockID { get; set; }
        public int ItemTypeID { get; set; }
        public string MedicineName { get; set; }
        public string Company { get; set; }
        public string BatchCode { get; set; }
        public string Diagnosis { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerQuantity { get; set; }
        public string ExpiryDate { get; set; }  // Changed to string
        public string EntryDate { get; set; }   // Changed to string
        public string DosageDetails { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
