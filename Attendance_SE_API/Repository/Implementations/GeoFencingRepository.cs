using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Attendance_SE_API.Repository.Implementations
{
    public class GeoFencingRepository : IGeoFencingRepository
    {
        private readonly IDbConnection _connection;

        public GeoFencingRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddGeoFancing(GeoFencingRequest request)
        {
            string query = @"INSERT INTO tblGeoFencingMaster (Latitude, Longitude, DepartmentID, RadiusInMeters, InstituteID) 
                             VALUES (@Latitude, @Longitude, @DepartmentID, @RadiusInMeters, @InstituteID);
                             SELECT CAST(SCOPE_IDENTITY() as int)";

            await _connection.ExecuteAsync(query, request);
            return new ServiceResponse<string>(true, "Geo-fencing entry added successfully.", null, 201);
        }

        public async Task<ServiceResponse<List<GetGeoFencingResponse>>> GetAllGeoFancing(PaginationRequest request)
        {
            // Update the query to include department name and support pagination
            string query = @"
    SELECT 
        g.GeoFencingID, 
        g.Latitude, 
        g.Longitude, 
        d.DepartmentName, 
        g.RadiusInMeters, 
        g.InstituteID, 
        g.IsActive 
    FROM 
        tblGeoFencingMaster g
    JOIN 
        tbl_Department d ON g.DepartmentID = d.Department_id
    WHERE 
        g.IsActive = 1
    ORDER BY 
        g.GeoFencingID
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                PageSize = request.PageSize
            };

            // Query the geo-fencing data
            var geoFencings = await _connection.QueryAsync<GetGeoFencingResponse>(query, parameters);

            // Get total count of active geo-fencing records for pagination
            string countQuery = "SELECT COUNT(*) FROM tblGeoFencingMaster WHERE IsActive = 1";
            var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery);

            // Create and return the ServiceResponse
            return new ServiceResponse<List<GetGeoFencingResponse>>(
                success: true,                       // First argument: success
                message: "Geo-fencing records retrieved successfully.", // Second argument: message
                data: geoFencings.AsList(),         // Third argument: data
                statusCode: 200,                     // Fourth argument: statusCode
                totalCount: totalCount               // Fifth argument: totalCount
            );
        }

        public async Task<ServiceResponse<GeoFencingResponse>> GetGeoFancing(int geoFencingID)
        {
            string query = "SELECT * FROM tblGeoFencingMaster WHERE GeoFencingID = @GeoFencingID";
            var geoFencing = await _connection.QueryFirstOrDefaultAsync<GeoFencingResponse>(query, new { GeoFencingID = geoFencingID });

            if (geoFencing != null)
            {
                return new ServiceResponse<GeoFencingResponse>(true, "Geo-fencing record found.", geoFencing, 200);
            }
            return new ServiceResponse<GeoFencingResponse>(false, "Geo-fencing record not found.", null, 404);
        }

        public async Task<ServiceResponse<bool>> DeleteGeoFancing(int geoFencingID)
        {
            string query = "UPDATE tblGeoFencingMaster SET IsActive = 0 WHERE GeoFencingID = @GeoFencingID";
            var rowsAffected = await _connection.ExecuteAsync(query, new { GeoFencingID = geoFencingID });

            return new ServiceResponse<bool>(true, "Geo-fencing entry deleted successfully.", rowsAffected > 0, 200);
        }
    }
}
