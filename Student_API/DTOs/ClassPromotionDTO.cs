namespace Student_API.DTOs
{
    public class ClassPromotionDTO
    {
        public List<ClassSectionDTO> ClassSections { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
        public int institute_id { get; set; }
    }

    public class ClassSectionDTO
    {
        public int OldClassId { get; set; }
        public int NewClassId { get; set; }
        public int OldSectionId { get; set; }
        public int NewSectionId { get; set; }
    }

}
