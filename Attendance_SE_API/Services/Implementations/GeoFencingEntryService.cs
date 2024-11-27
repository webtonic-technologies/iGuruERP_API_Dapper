using System.Threading.Tasks;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using System.Collections.Generic;
using Attendance_SE_API.Services.Interfaces;

namespace Attendance_SE_API.Services.Implementations
{
    public class GeoFencingEntryService : IGeoFencingEntryService
    {
        private readonly IGeoFencingEntryRepository _geoFencingEntryRepository;

        public GeoFencingEntryService(IGeoFencingEntryRepository geoFencingEntryRepository)
        {
            _geoFencingEntryRepository = geoFencingEntryRepository;
        }

        public async Task<ServiceResponse<string>> AddGeoFencingEntry(GeoFencingEntryRequest request)
        {
            var result = await _geoFencingEntryRepository.AddGeoFencingEntry(request);
            return result;
        }

        public async Task<ServiceResponse<IEnumerable<GeoFencingEntryResponse>>> GetGeoFencingEntry(GeoFencingEntryRequest2 request)
        {
            var result = await _geoFencingEntryRepository.GetGeoFencingEntry(request);
            return result;
        }
    }
}
