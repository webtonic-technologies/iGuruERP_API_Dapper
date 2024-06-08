﻿using Attendance_API.Models;
using Attendance_API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Services.Interfaces
{
    public interface IGeoFencingService
    {
        Task<ServiceResponse<GeoFencingResponseDTO>> GetGeoFencingById(int id);
        Task<ServiceResponse<GeoFencingResponseDTO>> GetAllGeoFencings(GeoFencingQueryParams request);
        Task<ServiceResponse<bool>> AddGeoFencing(GeoFencingDTO geoFencing);
        Task<ServiceResponse<bool>> UpdateGeoFencing(GeoFencingDTO geoFencing);
        Task<ServiceResponse<bool>> DeleteGeoFencing(int id);
    }
}
