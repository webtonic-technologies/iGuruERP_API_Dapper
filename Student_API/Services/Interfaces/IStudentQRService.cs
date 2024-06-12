﻿using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IStudentQRService
    {
        Task<ServiceResponse<List<StudentQRDTO>>> GetAllStudentQR(int section_id, int class_id, int? pageNumber = null, int? pageSize = null);
    }
}