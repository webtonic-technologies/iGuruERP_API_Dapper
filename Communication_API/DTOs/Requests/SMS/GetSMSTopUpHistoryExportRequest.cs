﻿namespace Communication_API.DTOs.Requests
{
    public class GetSMSTopUpHistoryExportRequest
    {
        public int InstituteID { get; set; }
        public int ExportType { get; set; } // 1 for Excel, 2 for CSV
    }
}
