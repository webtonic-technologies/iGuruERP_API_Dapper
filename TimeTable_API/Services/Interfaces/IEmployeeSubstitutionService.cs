﻿using System.Threading.Tasks;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;

namespace TimeTable_API.Services.Interfaces
{
    public interface IEmployeeSubstitutionService
    {
        Task<ServiceResponse<List<EmployeeSubstitutionResponse>>> GetSubstitution(EmployeeSubstitutionRequest request); // Ensure this matches your intent
        Task<ServiceResponse<int>> UpdateSubstitution(EmployeeSubstitutionRequest_Update request);
        Task<ServiceResponse<List<SubstituteEmployeeResponse>>> GetSubstituteEmployeeList(GetSubstituteEmployeeListRequest request);

    }
}
