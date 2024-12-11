using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Services.Implementations
{
    public class ConcessionService : IConcessionService
    {
        private readonly IConcessionRepository _concessionRepository;

        public ConcessionService(IConcessionRepository concessionRepository)
        {
            _concessionRepository = concessionRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateConcession(AddUpdateConcessionRequest request)
        {
            var result = await _concessionRepository.AddUpdateConcession(request);
            return new ServiceResponse<int>(true, "Concession group saved successfully", result, 200);
        }

        public async Task<ServiceResponse<IEnumerable<ConcessionResponse>>> GetAllConcessions(GetAllConcessionRequest request)
        {
            // Get the response from the repository
            var response = await _concessionRepository.GetAllConcessions(request);

            // Check if the repository response is successful
            if (response != null)
            {
                // Return a new ServiceResponse, extracting the Data from the repository response
                return new ServiceResponse<IEnumerable<ConcessionResponse>>(true, "Concession groups retrieved successfully", response.Data, 200, response.TotalCount);
            }

            // If there's an issue, return an appropriate error response
            return new ServiceResponse<IEnumerable<ConcessionResponse>>(false, "No concession groups found", null, 404);
        }


        public async Task<ServiceResponse<ConcessionResponse>> GetConcessionById(int concessionGroupID)
        {
            var result = await _concessionRepository.GetConcessionById(concessionGroupID);
            return new ServiceResponse<ConcessionResponse>(true, "Concession group retrieved successfully", result, 200);
        }

        //public async Task<int> UpdateConcessionGroupStatus(int concessionGroupID)
        //{
        //    return await _concessionRepository.UpdateConcessionGroupStatus(concessionGroupID);
        //}

        public async Task<int> UpdateConcessionGroupStatus(int concessionGroupID, string? inActiveReason)
        {
            return await _concessionRepository.UpdateConcessionGroupStatus(concessionGroupID, inActiveReason);
        }



    }
}
