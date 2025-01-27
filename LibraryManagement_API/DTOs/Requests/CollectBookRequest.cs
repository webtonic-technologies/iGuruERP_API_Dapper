namespace LibraryManagement_API.DTOs.Requests
{
    public class CollectBookRequest
    {
        public int CollectionID { get; set; }
        public int ReturnActionTypeID { get; set; }
        public int InstituteID { get; set; }
        public string ReturnDate { get; set; }  // String in DD-MM-YYYY format
        public string DueDate { get; set; }     // String in DD-MM-YYYY format
        public decimal LateFeeAmount { get; set; }
        public string Reason { get; set; }
        public string RevisedDueDate { get; set; } // String in DD-MM-YYYY format
        public decimal LostBookPenalty { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DamageBookPenalty { get; set; }
        public string BookCondition { get; set; }
    }
}
