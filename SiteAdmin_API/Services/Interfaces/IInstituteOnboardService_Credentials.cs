using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;

namespace SiteAdmin_API.Services.Interfaces
{
    public interface IInstituteOnboardService_Credentials
    {
        ServiceResponse<GenerateInstituteCredentialsResponse_Credentials> GenerateInstituteCredentials(string instituteName);
    }
}
