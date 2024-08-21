﻿namespace Infirmary_API.Models
{
    public class Medicine
    {
        public int MedicineID { get; set; }
        public int VisitID { get; set; }
        public int ItemTypeID { get; set; }
        public int PrescribedMedicineID { get; set; }
        public string NoOfDose { get; set; }
        public int Quantity { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
    }
}
