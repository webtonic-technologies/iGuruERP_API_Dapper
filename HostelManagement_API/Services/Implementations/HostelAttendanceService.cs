using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class HostelAttendanceService : IHostelAttendanceService
    {
        private readonly IHostelAttendanceRepository _hostelAttendanceRepository;

        public HostelAttendanceService(IHostelAttendanceRepository hostelAttendanceRepository)
        {
            _hostelAttendanceRepository = hostelAttendanceRepository;
        }

        public async Task<ServiceResponse<string>> SetHostelAttendance(SetHostelAttendanceRequest request)
        {
            return await _hostelAttendanceRepository.SetHostelAttendance(request);
        }

        public async Task<ServiceResponse<IEnumerable<GetHostelAttendanceResponse>>> GetHostelAttendance(GetHostelAttendanceRequest request)
        {
            var result = await _hostelAttendanceRepository.GetHostelAttendance(request);
            return new ServiceResponse<IEnumerable<GetHostelAttendanceResponse>>(true, "Hostel attendance retrieved successfully", result, 200);
        }

        public async Task<IEnumerable<GetHostelAttendanceTypeResponse>> GetHostelAttendanceTypes()
        {
            return await _hostelAttendanceRepository.GetHostelAttendanceTypes();
        }

        public async Task<IEnumerable<GetHostelAttendanceStatusResponse>> GetHostelAttendanceStatuses()
        {
            return await _hostelAttendanceRepository.GetHostelAttendanceStatuses();
        }
    }
}
