using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface INonAcademicFeeRepository
    {
        string AddNonAcademicFee(AddUpdateNonAcademicFeeRequest request);
        ServiceResponse<IEnumerable<GetNonAcademicFeeResponse>> GetNonAcademicFee(GetNonAcademicFeeRequest request);
        string DeleteNonAcademicFee(int nonAcademicFeesID);
        DataTable GetNonAcademicFeeExportData(GetNonAcademicFeeExportRequest request);
 
    }
}
