using Dapper;
using System.Data;
using Transport_API.DTOs.Response;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Repository.Interfaces;

namespace Transport_API.Repository.Implementations
{
    public class TransportSurveillanceRepository : ITransportSurveillanceRepository
    {

        private readonly IDbConnection _dbConnection;

        public TransportSurveillanceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<ServiceResponse<TransportSurveillance>> GetTransportSurveillanceById(int SurveillanceId, int InstituteId)
        {
            try
            {
                string query = @"
            SELECT ts.SurveillanceId, ts.VehicleId, v.VehicleNumber, ts.VideoFileLink, ts.InstituteId
            FROM tblTransportSurveillanceMaster ts
            JOIN tblVehicleMaster v ON ts.VehicleId = v.VehicleID
            WHERE ts.SurveillanceId = @SurveillanceId AND ts.InstituteId = @InstituteId";

                var surveillance = await _dbConnection.QueryFirstOrDefaultAsync<TransportSurveillance>(query, new { SurveillanceId, InstituteId });

                if (surveillance != null)
                {
                    return new ServiceResponse<TransportSurveillance>(true, "Transport Surveillance Retrieved Successfully", surveillance, 200);
                }

                return new ServiceResponse<TransportSurveillance>(false, "Transport Surveillance Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<TransportSurveillance>(false, ex.Message, null, 500);
            }
        }

    }
}
