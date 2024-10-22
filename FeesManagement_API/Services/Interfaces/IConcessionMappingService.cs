using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Interfaces
{
    public interface IConcessionMappingService
    {
        ServiceResponse<string> AddUpdateConcession(AddUpdateConcessionMappingRequest request);
        ServiceResponse<List<GetAllConcessionMappingResponse>> GetAllConcessionMapping(GetAllConcessionMappingRequest request);
        ServiceResponse<string> UpdateStatus(int studentConcessionID);
    }
}
