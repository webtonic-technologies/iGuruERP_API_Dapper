namespace Student_API.DTOs.RequestDTO
{
    public class PagedListModel
    {
        public string sortField {  get; set; }  
        public string sortDirection {  get; set; }
        public int? pageNumber { get; set; } = null;
        public int? pageSize { get; set; }  = null;
    }
}
