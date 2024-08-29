using Student_API.DTOs;
using Student_API.DTOs.RequestDTO;
using Student_API.DTOs.ServiceResponse;


namespace Student_API.Repository.Interfaces
{
    public interface IIMEIRegistrationRepo
    {
        Task<ServiceResponse<string>> AddIMEIRegistration(IMEIRegistrationModel imeiRegistrationDto);
        Task<ServiceResponse<List<IMEIRegistrationDto>>> GetAllIMEIRegistrations();
    }
}
