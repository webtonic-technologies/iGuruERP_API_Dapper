namespace Transport_API.DTOs.Response
{
    public class VehicleExpenseResponse
    {
        //public int VehicleExpenseId { get; set; }
        //public int ExpenseTypeId { get; set; }
        //public DateTime Date { get; set; }
        //public decimal Cost { get; set; }
        //public string Remarks { get; set; }


        //public int VehicleID { get; set; }
        //public string VehicleNumber { get; set; }
        //public string VehicleExpenseTypeName { get; set; }
        //public string TotalCost { get; set; }

        public int VehicleExpenseId { get; set; }
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }
        public string TotalCost { get; set; }
         
    }

    public class GetAllExpenseResponse
    {
        public int VehicleExpenseID { get; set; }
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }
        public string ExpenseType { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public List<string> Documents { get; set; }  // Holds paths of the documents
    }

}
