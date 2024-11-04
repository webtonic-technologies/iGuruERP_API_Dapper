﻿using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Repository.Interfaces
{
    public interface IGeoFencingRepository
    {
        Task<ServiceResponse<string>> AddGeoFancing(GeoFencingRequest request);
        Task<ServiceResponse<List<GetGeoFencingResponse>>> GetAllGeoFancing(PaginationRequest request);
        Task<ServiceResponse<GeoFencingResponse>> GetGeoFancing(int geoFencingID);
        Task<ServiceResponse<bool>> DeleteGeoFancing(int geoFencingID);
    }
}