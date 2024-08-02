namespace Student_API.DTOs
{
    public class PromoteStudentDTO
    {
       public  List<int> studentIds { get; set; }
        public int nextClassId { get; set; }    
        public int sectionId { get; set; }    
    }
}
