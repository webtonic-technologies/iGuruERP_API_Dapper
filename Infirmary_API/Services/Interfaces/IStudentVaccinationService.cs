using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Interfaces
{
    public interface IStudentVaccinationService
    {
        Task<ServiceResponse<string>> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request);
        Task<ServiceResponse<List<StudentVaccinationResponse>>> GetAllStudentVaccinations(GetAllStudentVaccinationsRequest request);
        Task<ServiceResponse<StudentVaccination>> GetStudentVaccinationById(int id);
        Task<ServiceResponse<bool>> DeleteStudentVaccination(int id);
    }
}
