﻿using System;
using System.Collections.Generic;

namespace Infirmary_API.Models
{
    public class InfirmaryVisit
    {
        public int VisitID { get; set; }
        public int VisitorTypeID { get; set; }
        public int VisitorID { get; set; }
        public DateTime EntryDate { get; set; }
        public TimeSpan EntryTime { get; set; }
        public DateTime? ExitDate { get; set; }
        public TimeSpan? ExitTime { get; set; }
        public string ReasonToVisitInfirmary { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Prescription { get; set; }
        public int DiagnosisBy { get; set; }
        public string PrescriptionFile { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }

        // Add this property to fix the error
        public List<Medicine> Medicines { get; set; }
    }
}
