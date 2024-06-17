using Attendance_API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Services.Interfaces
{
    public interface IShiftTimingService
    {
        Task<bool> AddShiftTimingAndDesignations(ShiftTimingRequestDTO request);
        Task<ShiftTimingResponseDTO> GetAllShiftTimings(ShiftTimingFilterDTO request);
        Task<ShiftTimingResponse> GetShiftTimingById(int id);
        Task<bool> EditShiftTimingAndDesignations(ShiftTimingRequestDTO request);
        Task<bool> DeleteShiftTiming(int id);
    }
}