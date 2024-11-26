using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Interfaces
{
    public interface IEmployeeShiftService
    {
        Task<ServiceResponse<string>> AddUpdateShift(List<ShiftRequest> requests);
        Task<ServiceResponse<List<ShiftResponse>>> GetAllShifts(GetAllShiftsRequest request);
        Task<ServiceResponse<ShiftResponse>> GetShiftById(int shiftID);
        Task<ServiceResponse<bool>> DeleteShift(int shiftID);
    }
}
