﻿using System.Threading.Tasks;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Response.StudentManagement;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;

namespace StudentManagement_API.Services.Interfaces
{
    public interface IStudentInformationService
    {
        Task<ServiceResponse<string>> AddUpdateStudent(AddUpdateStudentRequest request);
        // You can add other methods (e.g., GetStudent, DeleteStudent) here as needed.
        Task<ServiceResponse<IEnumerable<GetStudentInformationResponse>>> GetStudentInformation(GetStudentInformationRequest request);
        Task<ServiceResponse<string>> SetStudentStatusActivity(SetStudentStatusActivityRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentStatusActivityResponse>>> GetStudentStatusActivity(GetStudentStatusActivityRequest request);
        Task<ServiceResponse<byte[]>> DownloadStudentImportTemplate(int instituteID);
        Task<ServiceResponse<StudentImportResponse>> ImportStudentInformation(int instituteID, Stream fileStream); 
        Task<ServiceResponse<IEnumerable<GetStudentSettingResponse>>> GetStudentSetting(GetStudentSettingRequest request);
        Task<ServiceResponse<string>> AddRemoveStudentSetting(AddRemoveStudentSettingRequest request);

    }
}
 