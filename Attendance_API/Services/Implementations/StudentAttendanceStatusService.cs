using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;
using Attendance_API.Repository.Interfaces;
using Attendance_API.Services.Interfaces;

namespace Attendance_API.Services.Implementations
{
    public class StudentAttendanceStatusService : IStudentAttendanceStatusService
    {
        private readonly IStudentAttendanceStatusRepository _studentAttendanceStatusRepository;

        public StudentAttendanceStatusService(IStudentAttendanceStatusRepository studentAttendanceStatusRepository)
        {
            _studentAttendanceStatusRepository = studentAttendanceStatusRepository;
        }

        public async Task<ServiceResponse<List<StudentAttendanceStatusDTO>>> GetStudentAttendanceStatusList(int InstituteId)
        {
            try
            {
                return await _studentAttendanceStatusRepository.GetStudentAttendanceStatusList(InstituteId);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentAttendanceStatusDTO>>(false, ex.Message, new List<StudentAttendanceStatusDTO>(), 500);
            }
        }

        public async Task<ServiceResponse<StudentAttendanceStatusDTO>> GetStudentAttendanceStatusById(int Student_Attendance_Status_id)
        {
            try
            {
                return await _studentAttendanceStatusRepository.GetStudentAttendanceStatusById(Student_Attendance_Status_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<StudentAttendanceStatusDTO>(false, ex.Message, new StudentAttendanceStatusDTO(), 500);
            }
        }

        public async Task<ServiceResponse<string>> SaveStudentAttendanceStatus(List<StudentAttendanceStatusDTO> request)
        {
            try
            {
                return await _studentAttendanceStatusRepository.SaveStudentAttendanceStatus(request);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }

        //public async Task<ServiceResponse<string>> UpdateStudentAttendanceStatus(StudentAttendanceStatusDTO request)
        //{
        //    try
        //    {
        //        return await _studentAttendanceStatusRepository.UpdateStudentAttendanceStatus(request);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
        //    }
        //}

        public async Task<ServiceResponse<string>> DeleteStudentAttendanceStatus(int Student_Attendance_Status_id)
        {
            try
            {
                return await _studentAttendanceStatusRepository.DeleteStudentAttendanceStatus(Student_Attendance_Status_id);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, string.Empty, 500);
            }
        }
    }
}
