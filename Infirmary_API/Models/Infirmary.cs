namespace Infirmary_API.Models
{
    public class Infirmary
    {
        public int InfirmaryID { get; set; }
        public string InfirmaryName { get; set; }
        public int InfirmaryIncharge { get; set; }
        public int NoOfBeds { get; set; }
        public string Description { get; set; }
        public int InstituteID { get; set; }
        public bool IsActive { get; set; }
    }
}
