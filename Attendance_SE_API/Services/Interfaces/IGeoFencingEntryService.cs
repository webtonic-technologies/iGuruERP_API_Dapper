﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IGeoFencingEntryService
    {
        Task<ServiceResponse<string>> AddGeoFencingEntry(GeoFencingEntryRequest request);
        Task<ServiceResponse<IEnumerable<GeoFencingEntryResponse>>> GetGeoFencingEntry(GeoFencingEntryRequest2 request);
    }
}
