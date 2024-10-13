using Transport_API.DTOs.Requests;
using System;
using System.Collections.Generic;
using Transport_API.DTOs.Requests.Transport_API.DTOs.Requests;

public class VehicleExpense
{
    public int VehicleExpenseID { get; set; }
    public int ExpenseTypeID { get; set; }
    public DateTime ExpenseDate { get; set; } // Expect the format to be handled when parsing/serializing
    public decimal Cost { get; set; }
    public string Remarks { get; set; }
    public int VehicleID { get; set; }
    public int InstituteID { get; set; }

    // List to hold documents/attachments related to this expense
    public List<VehicleExpenseDocumentRequest> VehicleExpenseDocuments { get; set; }
}
