using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using System.Collections.Generic;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface INonAcademicFeeRepository
    {
        string AddNonAcademicFee(AddUpdateNonAcademicFeeRequest request);
        List<GetNonAcademicFeeResponse> GetNonAcademicFee(GetNonAcademicFeeRequest request);
        string DeleteNonAcademicFee(int nonAcademicFeesID);
    }
}
