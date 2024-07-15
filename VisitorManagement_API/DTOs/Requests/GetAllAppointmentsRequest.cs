﻿namespace VisitorManagement_API.DTOs.Requests
{
    public class GetAllAppointmentsRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int InstituteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate {  get; set; }
    }
}
