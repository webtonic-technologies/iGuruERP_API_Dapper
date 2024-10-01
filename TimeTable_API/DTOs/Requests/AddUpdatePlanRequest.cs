namespace TimeTable_API.DTOs.Requests
{
    public class AddUpdatePlanRequest
    {
        public int PlanID { get; set; } // PlanID column in tblTimeTableDaySetup
        public string PlanName { get; set; } // PlanName column in tblTimeTableDaySetup
        public string DayIDs { get; set; } // DayIDs (comma-separated DayID column from tblTimeTableDayMaster)
        public int InstituteID { get; set; } // InstituteID column in tblTimeTableDaySetup
        public List<GroupMapping> Groups { get; set; } // List of groups to be mapped
    }

    public class GroupMapping
    {
        public int GroupID { get; set; } // GroupID column in tblTimeTableGroups
        public int PlanID { get; set; } // PlanID column in tblTimeTableDaySetup
    }
}
