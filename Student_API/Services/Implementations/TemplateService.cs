using Student_API.DTOs;
using Student_API.DTOs.ServiceResponse;
using Student_API.Repository.Interfaces;
using Student_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Student_API.Services.Implementations
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _templateRepository;

        public TemplateService(ITemplateRepository templateRepository)
        {
            _templateRepository = templateRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateTemplate(TemplateDTO templateDto)
        {
            return await _templateRepository.AddUpdateTemplate(templateDto);
        }

        public async Task<ServiceResponse<TemplateDTO>> GetTemplateById(int templateId)
        {
            return await _templateRepository.GetTemplateById(templateId);
        }

        public async Task<ServiceResponse<bool>> DeleteTemplate(int templateId)
        {
            return await _templateRepository.DeleteTemplate(templateId);
        }

        public async Task<ServiceResponse<List<TemplateDTO>>> GetAllTemplates(int? pageSize = null, int? pageNumber = null)
        {
            return await _templateRepository.GetAllTemplates(pageSize, pageNumber);
        }
    }
}
