using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Interfaces
{
    public interface IShiftTimingRepository
    {
        Task<ServiceResponse<string>> AddOrEditShiftTimingsAndDesignations(List<ShiftTimingRequestDTO> requests);
        Task<ServiceResponse<ShiftTimingResponse>> GetShiftTimingById(int id);
        Task<ServiceResponse<string>> DeleteShiftTiming(int id);
        Task<ServiceResponse<ShiftTimingResponseDTO>> GetAllShiftTimings(ShiftTimingFilterDTO request);
    }
}