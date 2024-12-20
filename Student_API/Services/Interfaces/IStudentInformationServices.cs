﻿using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.Responses;
using Student_API.DTOs.ServiceResponse;
using Student_API.Services.Implementations;

namespace Student_API.Services.Interfaces
{
    public interface IStudentInformationServices
    {
        Task<ServiceResponse<int>> AddUpdateStudent(StudentDTO request);
        Task<ServiceResponse<int>> AddUpdateStudentInformation(StudentMasters request);
        Task<ServiceResponse<StudentInformationDTO>> GetStudentDetailsById(int studentId);
        Task<ServiceResponse<List<StudentDetailsDTO>>> GetAllStudentDetails(GetStudentRequestModel obj);
        Task<ServiceResponse<int>> ChangeStudentStatus(StudentStatusDTO statusDTO);
        Task<ServiceResponse<IEnumerable<StudentActivityHistoryResponse>>> GetStudentActivityHistory(int studentId, int instituteId);

        Task<ServiceResponse<int>> AddUpdateStudentOtherInfo(StudentOtherInfos request);
        Task<ServiceResponse<int>> AddUpdateStudentParentInfo(StudentParentInfo request);
        Task<ServiceResponse<int>> AddOrUpdateStudentSiblings(StudentSibling sibling);
        Task<ServiceResponse<int>> AddOrUpdateStudentPreviousSchool(StudentPreviousSchools previousSchool);
        Task<ServiceResponse<int>> AddOrUpdateStudentHealthInfo(StudentHealthInfos healthInfo);
        Task<ServiceResponse<int>> AddUpdateStudentDocuments(StudentDocumentsDTO request);
        Task<ServiceResponse<int>> DeleteStudentDocument(int Student_Documents_id);
        Task<ServiceResponse<List<StudentInformationDTO>>> GetAllStudentDetailsData(GetStudentRequestModel obj);
        Task<ServiceResponse<string>> GetAllStudentDetailsAsExcel(getStudentRequest obj);
        Task<ServiceResponse<int>> AddUpdateStudentSetting(StudentSettingDTO studentSettingDto);
        Task<ServiceResponse<List<StudentSettingDTO>>> GetStudentSettingByInstituteId(int instituteId);
        Task<ServiceResponse<List<dynamic>>> GetAllStudentDetailsData1(GetStudentRequestModel obj);
    }
}
