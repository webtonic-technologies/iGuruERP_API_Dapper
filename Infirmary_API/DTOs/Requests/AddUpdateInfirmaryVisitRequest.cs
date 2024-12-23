using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Infirmary_API.DTOs.Requests
{
    public class AddUpdateInfirmaryVisitRequest
    {
        public int VisitID { get; set; }
        public int VisitorTypeID { get; set; }
        public int VisitorID { get; set; }
        //public DateTime EntryDate { get; set; }
        //public TimeSpan EntryTime { get; set; }
        //public DateTime? ExitDate { get; set; }
        //public TimeSpan? ExitTime { get; set; }
        public string EntryDate { get; set; }  // Changed to string with format 'DD-MM-YYYY'
        public string EntryTime { get; set; }  // Changed to string with format 'hh:mm tt'
        public string ExitDate { get; set; }   // Changed to string with format 'DD-MM-YYYY'
        public string ExitTime { get; set; }   // Changed to string with format 'hh:mm tt'
        public string ReasonToVisitInfirmary { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Prescription { get; set; }
        public int DiagnosisBy { get; set; }
        public string PrescriptionFile { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<AddUpdateMedicineRequest>? Medicines { get; set; }
        public List<InfirmaryVisitDocs>? InfirmaryVisitDocs { get; set; }
    }
    public class InfirmaryVisitDocs
    {
        public int DocumentsId { get; set; }
        public int VisitID { get; set; }
        public string DocFile { get; set; } = string.Empty;
    }
}
