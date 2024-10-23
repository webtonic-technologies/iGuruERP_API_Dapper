using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using System.Collections.Generic;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IConcessionMappingRepository
    {
        string AddUpdateConcession(AddUpdateConcessionMappingRequest request);
        List<GetAllConcessionMappingResponse> GetAllConcessionMapping(GetAllConcessionMappingRequest request);
        string UpdateStatus(int studentConcessionID);
    }
}
