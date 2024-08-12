using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.Models;
using Attendance_API.DTOs;
using Attendance_API.Repository.Interfaces;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class GeoFencingService : IGeoFencingService
    {
        private readonly IGeoFencingRepository _geoFencingRepository;

        public GeoFencingService(IGeoFencingRepository geoFencingRepository)
        {
            _geoFencingRepository = geoFencingRepository;
        }

        public async Task<ServiceResponse<GeoFencingResponseDTO>> GetGeoFencingById(int id)
        {
            return await _geoFencingRepository.GetGeoFencingById(id);
        }

        public async Task<ServiceResponse<GeoFencingResponseDTO>> GetAllGeoFencings(GeoFencingQueryParams request)
        {
            return await _geoFencingRepository.GetAllGeoFencings(request);
        }

        public async Task<ServiceResponse<bool>> AddOrUpdateGeoFencing(List<GeoFencingDTO> geoFencing)
        {
            return await _geoFencingRepository.AddOrUpdateGeoFencing(geoFencing);
        }

        public async Task<ServiceResponse<bool>> DeleteGeoFencing(int id)
        {
            return await _geoFencingRepository.DeleteGeoFencing(id);
        }
    }
}
