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

        //public async Task<ServiceResponse<string>> AddGeoFancing(GeoFencingRequest request)
        //{
        //    string query = @"INSERT INTO tblGeoFencingMaster (Latitude, Longitude, DepartmentID, RadiusInMeters, InstituteID) 
        //                     VALUES (@Latitude, @Longitude, @DepartmentID, @RadiusInMeters, @InstituteID);
        //                     SELECT CAST(SCOPE_IDENTITY() as int)";

        //    await _connection.ExecuteAsync(query, request);
        //    return new ServiceResponse<string>(true, "Geo-fencing entry added successfully.", null, 201);
        //}

        public async Task<ServiceResponse<string>> AddGeoFancing(List<GeoFencingRequest> requests)
        {
            try
            {
                // Open the connection synchronously (works for all connection types)
                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    _connection.Open(); // Synchronous open
                }

                // Start a transaction to ensure atomicity if you are inserting multiple records.
                using (var transaction = _connection.BeginTransaction())
                {
                    foreach (var request in requests)
                    {
                        string query = @"INSERT INTO tblGeoFencingMaster (Latitude, Longitude, DepartmentID, RadiusInMeters, InstituteID) 
                                 VALUES (@Latitude, @Longitude, @DepartmentID, @RadiusInMeters, @InstituteID);
                                 SELECT CAST(SCOPE_IDENTITY() as int)";  // Capture the generated ID.

                        // Execute the query for each geo-fencing entry in the list
                        await _connection.ExecuteAsync(query, request, transaction);
                    }

                    // Commit the transaction if all inserts succeed
                    transaction.Commit();
                }

                return new ServiceResponse<string>(true, "Geo-fencing entries added successfully.", null, 201);
            }
            catch (Exception ex)
            {
                // Rollback transaction if there is any error
                return new ServiceResponse<string>(false, $"Error adding geo-fencing entries: {ex.Message}", null, 500);
            }
        }





        public async Task<ServiceResponse<List<GetGeoFencingResponse>>> GetAllGeoFancing(PaginationRequest request)
        {
            try
            {
                // Define the query to retrieve geo-fencing data with pagination and filter by InstituteID
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
                        AND g.InstituteID = @InstituteID
                    ORDER BY 
                        g.GeoFencingID
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                // Prepare parameters including InstituteID, pagination
                var parameters = new
                {
                    InstituteID = request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,  // Calculate the offset
                    PageSize = request.PageSize
                };

                // Execute the query to retrieve geo-fencing data
                var geoFencings = await _connection.QueryAsync<GetGeoFencingResponse>(query, parameters);

                // Query to get the total count of active geo-fencing records for pagination
                string countQuery = "SELECT COUNT(*) FROM tblGeoFencingMaster WHERE IsActive = 1 AND InstituteID = @InstituteID";
                var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new { InstituteID = request.InstituteID });

                // Return the results in a ServiceResponse
                return new ServiceResponse<List<GetGeoFencingResponse>>(
                    success: true,
                    message: "Geo-fencing records retrieved successfully.",
                    data: geoFencings.ToList(),
                    statusCode: 200,
                    totalCount: totalCount  // Return total count for pagination
                );
            }
            catch (Exception ex)
            {
                // Handle errors and return a failure response
                return new ServiceResponse<List<GetGeoFencingResponse>>(
                    success: false,
                    message: $"Error retrieving geo-fencing records: {ex.Message}",
                    data: null,
                    statusCode: 500,
                    totalCount: null  // No total count in case of error
                );
            }
        }

        public async Task<ServiceResponse<GeoFencingResponse>> GetGeoFancing(int geoFencingID)
        {
            string query = "SELECT * FROM tblGeoFencingMaster WHERE GeoFencingID = @GeoFencingID AND IsActive = 1";
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
