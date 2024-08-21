using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Models;
using Infirmary_API.Repository.Interfaces;
using Infirmary_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infirmary_API.Services.Implementations
{
    public class StudentVaccinationService : IStudentVaccinationService
    {
        private readonly IStudentVaccinationRepository _studentVaccinationRepository;

        public StudentVaccinationService(IStudentVaccinationRepository studentVaccinationRepository)
        {
            _studentVaccinationRepository = studentVaccinationRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateStudentVaccination(AddUpdateStudentVaccinationRequest request)
        {
            return await _studentVaccinationRepository.AddUpdateStudentVaccination(request);
        }

        public async Task<ServiceResponse<List<StudentVaccinationResponse>>> GetAllStudentVaccinations(GetAllStudentVaccinationsRequest request)
        {
            return await _studentVaccinationRepository.GetAllStudentVaccinations(request);
        }

        public async Task<ServiceResponse<StudentVaccination>> GetStudentVaccinationById(int id)
        {
            return await _studentVaccinationRepository.GetStudentVaccinationById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteStudentVaccination(int id)
        {
            return await _studentVaccinationRepository.DeleteStudentVaccination(id);
        }
    }
}
