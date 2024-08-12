using Attendance_API.DTOs;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
using Attendance_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Services.Implementations
{
    public class ShiftTimingService : IShiftTimingService
    {
        private readonly IShiftTimingRepository _shiftTimingRepository;

        public ShiftTimingService(IShiftTimingRepository shiftTimingRepository)
        {
            _shiftTimingRepository = shiftTimingRepository;
        }

        public async Task<ServiceResponse<string>> AddShiftTimingAndDesignations(ShiftTimingRequestDTO request)
        {
            return await _shiftTimingRepository.AddShiftTimingAndDesignations(request);
        }

        public async Task<ServiceResponse<ShiftTimingResponse>> GetShiftTimingById(int id)
        {
            return await _shiftTimingRepository.GetShiftTimingById(id);
        }

        public async Task<ServiceResponse<string>> EditShiftTimingAndDesignations(ShiftTimingRequestDTO request)
        {
            return await _shiftTimingRepository.EditShiftTimingAndDesignations(request);
        }

        public async Task<ServiceResponse<string>> DeleteShiftTiming(int id)
        {
            return await _shiftTimingRepository.DeleteShiftTiming(id);
        }

        public async Task<ServiceResponse<ShiftTimingResponseDTO>> GetAllShiftTimings(ShiftTimingFilterDTO request)
        {
            return await _shiftTimingRepository.GetAllShiftTimings(request);
        }
    }
}
