﻿using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Repository.Interfaces
{
    public interface IStudentQRRepository
    {
        Task<ServiceResponse<List<StudentQRDTO>>> GetAllStudentQR(int sectionId, int classId, string sortField, string sortDirection, int? pageNumber = null, int? pageSize = null);
    }
}
