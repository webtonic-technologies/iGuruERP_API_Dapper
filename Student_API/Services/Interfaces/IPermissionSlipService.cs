﻿using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface IPermissionSlipService
    {
        Task<ServiceResponse<List<PermissionSlipDTO>>> GetAllPermissionSlips(int classId, int sectionId, int? pageNumber = null, int? pageSize = null);
        Task<ServiceResponse<string>> UpdatePermissionSlipStatus(int permissionSlipId, bool isApproved);
    }
}
