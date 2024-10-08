namespace Employee_API.Models
{
    public class EmployeeFamily
    {
        public int? Employee_family_id { get; set; }
        public int? Employee_id { get; set; }
        public string Father_Name { get; set; } = string.Empty;
        public string Fathers_Occupation { get; set; } = string.Empty;
        public string Mother_Name { get; set; } = string.Empty;
        public string Mothers_Occupation { get; set; } = string.Empty;
        public string Spouse_Name { get; set; } = string.Empty;
        public string Spouses_Occupation { get; set; } = string.Empty;
        public string Guardian_Name { get; set; } = string.Empty;
        public string Guardians_Occupation { get; set; } = string.Empty;
        public string Primary_Emergency_Contact_no { get; set; } = string.Empty;
        public string Secondary_Emergency_Contact_no { get; set; } = string.Empty;
    }
}
