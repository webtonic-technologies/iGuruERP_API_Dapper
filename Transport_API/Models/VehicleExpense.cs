using Transport_API.DTOs.Requests;

public class VehicleExpense
{
    public int VehicleExpenseID { get; set; }
    public int ExpenseTypeID { get; set; }
    public DateTime ExpenseDate { get; set; }
    public decimal Cost { get; set; }
    public string Remarks { get; set; }
    public int VehicleID { get; set; }
    public int InstituteID { get; set; }

    public List<VehicleExpenseDocumentRequest> VehicleExpenseDocuments { get; set; }


    //public int VehicleID { get; set; }
    //public string VehicleModel { get; set; }
    //public string Fuel_type_name { get; set; }
    //public string TotalCost { get; set; }
}



