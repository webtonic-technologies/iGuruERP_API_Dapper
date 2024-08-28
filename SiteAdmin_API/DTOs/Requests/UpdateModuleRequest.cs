﻿namespace SiteAdmin_API.DTOs.Requests
{
    public class UpdateModuleRequest
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int ModuleOrder { get; set; }
        public bool IsActive { get; set; }
    }
}