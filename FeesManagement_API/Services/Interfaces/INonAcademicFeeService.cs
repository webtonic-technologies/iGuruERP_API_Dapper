using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;

namespace FeesManagement_API.Services.Interfaces
{
    public interface INonAcademicFeeService
    {
        ServiceResponse<string> AddNonAcademicFee(AddUpdateNonAcademicFeeRequest request);
        ServiceResponse<List<GetNonAcademicFeeResponse>> GetNonAcademicFee(GetNonAcademicFeeRequest request);

        ServiceResponse<string> DeleteNonAcademicFee(int nonAcademicFeesID);
        byte[] GetNonAcademicFeeExport(GetNonAcademicFeeExportRequest request);

    }
}
