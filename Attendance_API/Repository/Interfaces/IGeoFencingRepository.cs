using Attendance_API.Models;
using Attendance_API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IGeoFencingRepository
    {
        Task<ServiceResponse<GeoFencingResponseDTO>> GetGeoFencingById(int id);
        Task<ServiceResponse<GeoFencingResponseDTO>> GetAllGeoFencings(GeoFencingQueryParams request);
        Task<ServiceResponse<bool>> AddOrUpdateGeoFencing(List<GeoFencingDTO> geoFencings);
        Task<ServiceResponse<bool>> DeleteGeoFencing(int id);
    }
}