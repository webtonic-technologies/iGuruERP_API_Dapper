using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface INumberSchemeRepository
    {
        Task<int> AddUpdateNumberScheme(AddUpdateNumberSchemeRequest request);
        Task<IEnumerable<NumberSchemeResponse>> GetAllNumberSchemes(GetAllNumberSchemesRequest request);
        Task<NumberSchemeResponse> GetNumberSchemeById(int numberSchemeID);
        Task<int> UpdateNumberSchemeStatus(int numberSchemeID);
        Task<IEnumerable<NumberSchemeTypeResponse>> GetNumberSchemeType();
        Task<int> CountActiveNumberSchemes(int instituteID); // Add this line





    }
}
