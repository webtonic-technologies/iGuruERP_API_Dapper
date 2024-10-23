using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Implementations
{
    public class GeoFencingService : IGeoFencingService
    {
        private readonly IGeoFencingRepository _repository;

        public GeoFencingService(IGeoFencingRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddGeoFancing(GeoFencingRequest request)
        {
            return await _repository.AddGeoFancing(request);
        }

        public async Task<ServiceResponse<List<GetGeoFencingResponse>>> GetAllGeoFancing(PaginationRequest request)
        {
            return await _repository.GetAllGeoFancing(request);
        }

        public async Task<ServiceResponse<GeoFencingResponse>> GetGeoFancing(int geoFencingID)
        {
            return await _repository.GetGeoFancing(geoFencingID);
        }

        public async Task<ServiceResponse<bool>> DeleteGeoFancing(int geoFencingID)
        {
            return await _repository.DeleteGeoFancing(geoFencingID);
        }
    }
}
