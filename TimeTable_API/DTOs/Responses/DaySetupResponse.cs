namespace TimeTable_API.DTOs.Responses
{
    public class DaySetupResponse
    {
        public int PlanID { get; set; } // PlanID column in tblTimeTableDaySetup
        public string PlanName { get; set; } // PlanName column in tblTimeTableDaySetup
        public List<GroupMappingResponse> MappedTo { get; set; } // List of group mappings
    }

    public class GroupMappingResponse
    {
        public int GroupID { get; set; } // GroupID column in tblTimeTableGroups
        public string GroupName { get; set; } // GroupName column in tblTimeTableGroups
    }
}
