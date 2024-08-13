using System;
using System.Collections.Generic;

namespace Infirmary_API.DTOs.Requests
{
    public class AddUpdateStudentVaccinationRequest
    {
        public int StudentVaccinationID { get; set; } // If updating, this will have a value
        public string AcademicYear { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int VaccinationID { get; set; }
        public DateTime DateOfVaccination { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
        public List<int> StudentIDs { get; set; } // List of student IDs to be associated with this vaccination
    }
}
