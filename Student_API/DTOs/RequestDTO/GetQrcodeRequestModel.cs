namespace Student_API.DTOs.RequestDTO
{
    public class GetQrcodeRequestModel : PagedListModel
    {
        public int section_id {  get; set; }    
        public int class_id {  get; set; }    
        public string searchQuery  {  get; set; }    
    }
}
