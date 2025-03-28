﻿using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Repository.Interfaces
{
    public interface IStudentVaccinationRepository
    {
        Task<ServiceResponse<string>> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request);
        Task<ServiceResponse<List<StudentVaccinationResponse>>> GetAllStudentVaccinations(GetAllStudentVaccinationsRequest request);
        Task<ServiceResponse<List<StudentVaccinationResponse>>> GetStudentVaccinationById(int id);
        Task<ServiceResponse<bool>> DeleteStudentVaccination(int id);
        Task<List<GetStudentVaccinationsExportResponse>> GetStudentVaccinationsData(int instituteId, int classId, int sectionId, int vaccinationId);

    }
}
