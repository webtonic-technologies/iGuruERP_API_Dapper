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
    public class InfirmaryVisitService : IInfirmaryVisitService
    {
        private readonly IInfirmaryVisitRepository _infirmaryVisitRepository;

        public InfirmaryVisitService(IInfirmaryVisitRepository infirmaryVisitRepository)
        {
            _infirmaryVisitRepository = infirmaryVisitRepository;
        }

        public async Task<ServiceResponse<string>> AddUpdateInfirmaryVisit(AddUpdateInfirmaryVisitRequest request)
        {
            return await _infirmaryVisitRepository.AddUpdateInfirmaryVisit(request);
        }

        public async Task<ServiceResponse<List<InfirmaryVisitResponse>>> GetAllInfirmaryVisits(GetAllInfirmaryVisitsRequest request)
        {
            return await _infirmaryVisitRepository.GetAllInfirmaryVisits(request);
        }

        public async Task<ServiceResponse<InfirmaryVisit>> GetInfirmaryVisitById(int id)
        {
            return await _infirmaryVisitRepository.GetInfirmaryVisitById(id);
        }

        public async Task<ServiceResponse<bool>> DeleteInfirmaryVisit(int id)
        {
            return await _infirmaryVisitRepository.DeleteInfirmaryVisit(id);
        }
    }
}
