using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Implementations
{
    public class ConcessionMappingService : IConcessionMappingService
    {
        private readonly IConcessionMappingRepository _repository;

        public ConcessionMappingService(IConcessionMappingRepository repository)
        {
            _repository = repository;
        }

        public ServiceResponse<string> AddUpdateConcession(AddUpdateConcessionMappingRequest request)
        {
            var result = _repository.AddUpdateConcession(request);
            return new ServiceResponse<string>(true, "Concession added/updated successfully", result, 200);
        }

        public ServiceResponse<List<GetAllConcessionMappingResponse>> GetAllConcessionMapping(GetAllConcessionMappingRequest request)
        {
            var result = _repository.GetAllConcessionMapping(request);
            return new ServiceResponse<List<GetAllConcessionMappingResponse>>(true, "Concessions retrieved successfully", result, 200, result.Count);
        }

        public ServiceResponse<string> UpdateStatus(int studentConcessionID)
        {
            var result = _repository.UpdateStatus(studentConcessionID);
            return new ServiceResponse<string>(true, "Status updated successfully", result, 200);
        }
    }
}
