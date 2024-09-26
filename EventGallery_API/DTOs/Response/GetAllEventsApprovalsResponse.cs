namespace EventGallery_API.DTOs.Responses
{
    public class GetAllEventsApprovalsResponse
    {
        public int EventID { get; set; } // EventID column from tblEvent
        public string EventName { get; set; } // EventName column from tblEvent
        public string Date { get; set; } // 'FromDate to ToDate' formatted as 'DD-MM-YYYY to DD-MM-YYYY'
        public string Description { get; set; } // Description column from tblEvent
        public string Document { get; set; } = ""; // Default empty as per response
        public string Location { get; set; } // Location column from tblEvent
        public string EventNotification { get; set; } // 'ScheduleDate (DD-MM-YYYY) at ScheduleTime AM/PM'
        public string CreatedBy { get; set; } // 'First_Name + Last_Name' from tbl_EmployeeProfileMaster
        public int StatusID { get; set; } // StatusID column from tblEvent
    }
}
