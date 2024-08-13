using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_API.Services.Interfaces
{
    public interface IShiftTimingService
    {
        Task<ServiceResponse<string>> AddOrEditShiftTimingsAndDesignations(List<ShiftTimingRequestDTO> requests);
        Task<ServiceResponse<ShiftTimingResponseDTO>> GetAllShiftTimings(ShiftTimingFilterDTO request);
        Task<ServiceResponse<ShiftTimingResponse>> GetShiftTimingById(int id);
        Task<ServiceResponse<string>> DeleteShiftTiming(int id);
    }
}