using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;

namespace Student_API.Services.Interfaces
{
    public interface ITemplateService
    {
        Task<ServiceResponse<int>> AddUpdateTemplate(TemplateDTO templateDto);
        Task<ServiceResponse<TemplateDTO>> GetTemplateById(int templateId);
        Task<ServiceResponse<bool>> DeleteTemplate(int templateId);
        Task<ServiceResponse<List<TemplateDTO>>> GetAllTemplates(int? pageSize = null, int? pageNumber = null);
    }
}
