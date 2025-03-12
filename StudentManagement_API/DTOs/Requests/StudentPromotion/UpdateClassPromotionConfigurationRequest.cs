namespace StudentManagement_API.DTOs.Requests
{
    public class UpdateClassPromotionConfigurationRequest
    {
        public int InstituteID { get; set; }
        public int UserID { get; set; }
        public string IPAddress { get; set; }
        public List<ClassPromotionConfiguration> Classes { get; set; }
    }

    public class ClassPromotionConfiguration
    {
        public int CurrentClassID { get; set; }
        public int PromotedClassID { get; set; }
    }
}
