using Attendance_API.DTOs;

namespace Attendance_API.Repository.Interfaces
{
    public interface IShiftTimingRepository
    {
        Task<bool> AddShiftTimingAndDesignations(ShiftTimingRequestDTO request);
        Task<ShiftTimingResponse> GetShiftTimingById(int id);
        Task<bool> EditShiftTimingAndDesignations(ShiftTimingRequestDTO request);
        Task<bool> DeleteShiftTiming(int id);
        Task<ShiftTimingResponseDTO> GetAllShiftTimings(ShiftTimingFilterDTO request);
    }
}