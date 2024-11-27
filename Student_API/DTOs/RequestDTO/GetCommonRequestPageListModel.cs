namespace Student_API.DTOs.RequestDTO
{
    public class GetCommonRequestPageListModel : PagedListModel
    {
        public int Institute_id {  get; set; }  
        public string searchQuery  {  get; set; }  
    }
}
