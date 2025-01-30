namespace HostelManagement_API.DTOs.Requests
{
    public class AllocateHostelRequest
    { 
        public int StudentID { get; set; }
        public int HostelID { get; set; }
        public int RoomID { get; set; }
        public int RoomBedID { get; set; }
        public string? AllocateDate { get; set; }
        public string? VacateDate { get; set; }
        public bool IsAllocated { get; set; }
        public bool IsVacated { get; set; }
        public int InstituteID { get; set; }
        public int AllotterID { get; set; }
    }
}
