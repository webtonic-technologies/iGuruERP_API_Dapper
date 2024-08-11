using Attendance_API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs.ServiceResponse;


namespace Attendance_API.Services.Interfaces
{
    public interface IShiftTimingService
    {
        Task<ServiceResponse<string>> AddShiftTimingAndDesignations(ShiftTimingRequestDTO request);
        Task<ServiceResponse<ShiftTimingResponseDTO>> GetAllShiftTimings(ShiftTimingFilterDTO request);
        Task<ServiceResponse<ShiftTimingResponse>> GetShiftTimingById(int id);
        Task<ServiceResponse<string>> EditShiftTimingAndDesignations(ShiftTimingRequestDTO request);
        Task<ServiceResponse<string>> DeleteShiftTiming(int id);
    }
}