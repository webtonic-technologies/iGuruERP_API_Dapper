using Attendance_API.DTOs;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;
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

        public async Task<bool> AddShiftTimingAndDesignations(ShiftTimingRequestDTO request)
        {
            return await _shiftTimingRepository.AddShiftTimingAndDesignations(request);
        }

        public async Task<List<ShiftTimingResponseDTO>> GetAllShiftTimings()
        {
            return await _shiftTimingRepository.GetAllShiftTimings();
        }

        public async Task<ShiftTimingResponseDTO> GetShiftTimingById(int id)
        {
            return await _shiftTimingRepository.GetShiftTimingById(id);
        }

        public async Task<bool> EditShiftTimingAndDesignations(ShiftTimingRequestDTO request)
        {
            return await _shiftTimingRepository.EditShiftTimingAndDesignations(request);
        }

        public async Task<bool> DeleteShiftTiming(int id)
        {
            return await _shiftTimingRepository.DeleteShiftTiming(id);
        }
    }
}
