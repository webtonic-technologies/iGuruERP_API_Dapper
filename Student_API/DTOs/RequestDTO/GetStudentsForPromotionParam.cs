namespace Student_API.DTOs.RequestDTO
{
    public class GetStudentsForPromotionParam :PagedListModel
    {
        public int classId { get; set; }
    }
}
