using Attendance_SE_API.DTOs.Requests; // Use the correct namespace for ShiftRequest
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Models; // Ensure Shift is imported from the correct namespace
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Attendance_SE_API.Services.Implementations
{
    public class EmployeeShiftService : IEmployeeShiftService
    {
        private readonly IEmployeeShiftRepository _repository;

        public EmployeeShiftService(IEmployeeShiftRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<string>> AddUpdateShift(DTOs.Requests.ShiftRequest request)
        {
            // If ShiftRequest contains Designation data, handle it accordingly
            return await _repository.AddUpdateShift(request);
        }

        public async Task<ServiceResponse<List<ShiftResponse>>> GetAllShifts(GetAllShiftsRequest request)
        {
            // Get all shifts based on InstituteID with pagination
            return await _repository.GetAllShifts(request);
        }


        public async Task<ServiceResponse<ShiftResponse>> GetShiftById(int shiftID)
        {
            return await _repository.GetShiftById(shiftID);
        }

        public async Task<ServiceResponse<bool>> DeleteShift(int shiftID)
        {
            return await _repository.DeleteShift(shiftID);
        }
    }
}
