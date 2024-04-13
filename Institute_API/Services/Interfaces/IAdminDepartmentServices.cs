﻿using Institute_API.DTOs.ServiceResponse;
using Institute_API.Models;

namespace Institute_API.Services.Interfaces
{
    public interface IAdminDepartmentServices
    {
        Task<ServiceResponse<string>> AddUpdateAdminDept(AdminDepartment request);
        Task<ServiceResponse<string>> DeleteAdminDepartment(int Department_id);
        Task<ServiceResponse<AdminDepartment>> GetAdminDepartmentById(int Department_id);
        Task<ServiceResponse<List<AdminDepartment>>> GetAdminDepartmentList(int Institute_id);
    }
}
