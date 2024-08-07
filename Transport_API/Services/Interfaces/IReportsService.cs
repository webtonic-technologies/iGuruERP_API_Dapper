﻿using Transport_API.DTOs.Requests;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Services.Interfaces
{
    public interface IReportsService
    {
        Task<ServiceResponse<IEnumerable<ReportResponse>>> GetTransportPendingFeeReport(GetReportsRequest request);
        Task<ServiceResponse<IEnumerable<ReportResponse>>> GetEmployeeTransportationReport(GetReportsRequest request);
        Task<ServiceResponse<IEnumerable<ReportResponse>>> GetStudentTransportReport(GetReportsRequest request);
        Task<ServiceResponse<IEnumerable<ReportResponse>>> GetDeAllocationReport(GetReportsRequest request);
    }
}
