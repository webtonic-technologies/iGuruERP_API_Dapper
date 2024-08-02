using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;

namespace Transport_API.Repository.Interfaces
{
    public interface ITransportSurveillanceRepository
    {
        Task<ServiceResponse<TransportSurveillance>> GetTransportSurveillanceById(int SurveillanceId, int InstituteId);
    }
}
