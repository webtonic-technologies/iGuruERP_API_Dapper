using Communication_API.DTOs.Requests.SMS;
using Communication_API.DTOs.ServiceResponse;
using Communication_API.Models.SMS;
using Communication_API.Repository.Interfaces.SMS;
using Communication_API.Services.Interfaces.SMS;

namespace Communication_API.Services.Implementations.SMS
{
    public class SMSService : ISMSService
    {
        private readonly ISMSRepository _smsRepository;

        public SMSService(ISMSRepository smsRepository)
        {
            _smsRepository = smsRepository;
        }

        public async Task<ServiceResponse<string>> SetupSMSConfiguration(SetupSMSConfigurationRequest request)
        {
            return await _smsRepository.SetupSMSConfiguration(request);
        }

        public async Task<ServiceResponse<SMSBalance>> GetSMSBalance(int VendorID)
        {
            return await _smsRepository.GetSMSBalance(VendorID);
        }

        public async Task<ServiceResponse<string>> CreateSMSTemplate(CreateSMSTemplateRequest request)
        {
            return await _smsRepository.CreateSMSTemplate(request);
        }

        public async Task<ServiceResponse<List<SMSTemplate>>> GetAllSMSTemplate(GetAllSMSTemplateRequest request)
        {
            return await _smsRepository.GetAllSMSTemplate(request);
        }

        public async Task<ServiceResponse<string>> SendNewSMS(SendNewSMSRequest request)
        {
            return await _smsRepository.SendNewSMS(request);
        }

        public async Task<ServiceResponse<List<SMSReport>>> GetSMSReport(GetSMSReportRequest request)
        {
            return await _smsRepository.GetSMSReport(request);
        }
    }
}
