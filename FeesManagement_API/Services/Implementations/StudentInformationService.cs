using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Implementations
{
    public class StudentInformationService : IStudentInformationService
    {
        private readonly IStudentInformationRepository _studentInformationRepository;

        public StudentInformationService(IStudentInformationRepository studentInformationRepository)
        {
            _studentInformationRepository = studentInformationRepository;
        }

        //public ServiceResponse<StudentInformationResponse> GetStudentInformation(StudentInformationRequest request)
        //{
        //    return _studentInformationRepository.GetStudentInformation(request);
        //}

        public ServiceResponse<List<StudentInformationResponse>> GetStudentInformation(StudentInformationRequest request)
        {
            return _studentInformationRepository.GetStudentInformation(request);
        }

    }
}
