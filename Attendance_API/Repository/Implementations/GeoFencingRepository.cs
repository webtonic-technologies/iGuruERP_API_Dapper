using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Attendance_API.Models;
using Attendance_API.Repository.Interfaces;
using Attendance_API.DTOs;
using Attendance_API.DTOs.ServiceResponse;

namespace Attendance_API.Repository.Implementations
{
    public class GeoFencingRepository : IGeoFencingRepository
    {
        private readonly IDbConnection _dbConnection;

        public GeoFencingRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<GeoFencingResponseDTO>> GetGeoFencingById(int id)
        {
            var query = "SELECT g.*, d.DepartmentName as Department_Name FROM tbl_GeoFencing g JOIN tbl_Department d ON g.Department_id = d.Department_id WHERE Geo_Fencing_id = @Id AND g.IsDeleted = 0";
            var parameters = new { Id = id };
            var geoFencing = await _dbConnection.QueryFirstOrDefaultAsync<GeoFencingResponseDTO>(query, parameters);
            return new ServiceResponse<GeoFencingResponseDTO>(true, "GeoFencing found", geoFencing, 200);
        }

        public async Task<ServiceResponse<GeoFencingResponseDTO>> GetAllGeoFencings(GeoFencingQueryParams request)
        {
            var query = $"SELECT g.*, d.DepartmentName as Department_Name FROM tbl_GeoFencing g JOIN tbl_Department d ON g.Department_id = d.Department_id WHERE InstituteId = {request.InstituteId} AND g.IsDeleted = 0";
            if(request.pageNumber != null && request.pageSize != null)
            {
                query += $" Order by 1 OFFSET {(request.pageNumber - 1) * request.pageSize} ROWS FETCH NEXT {request.pageSize} ROWS ONLY;";
            }

            var geoFencings = await _dbConnection.QueryAsync<GeoFencingResponse>(query);
            query = $"SELECT COUNT(*) FROM tbl_GeoFencing g JOIN tbl_Department d ON g.Department_id = d.Department_id WHERE g.IsDeleted = 0 AND InstituteId = {request.InstituteId}";
            var countRes = await _dbConnection.QueryAsync<long>(query);
            var count = countRes.FirstOrDefault();
            return new ServiceResponse<GeoFencingResponseDTO>(true, "All GeoFencings found", new GeoFencingResponseDTO { Data = geoFencings, Total = count }, 200);
        }

        public async Task<ServiceResponse<bool>> AddOrUpdateGeoFencing(List<GeoFencingDTO> geoFencings)
        {
            foreach (var geoFencing in geoFencings)
            {
                var checkQuery = "SELECT COUNT(1) FROM tbl_GeoFencing WHERE Geo_Fencing_id = @Geo_Fencing_id";
                var checkParameters = new { Geo_Fencing_id = geoFencing.Geo_Fencing_id };
                var recordCount = await _dbConnection.ExecuteScalarAsync<int>(checkQuery, checkParameters);

                if (recordCount == 0)
                {
                    var insertQuery = "INSERT INTO tbl_GeoFencing (Latitude, Longitude, Department_id, Radius_In_Meters, Search_Location, InstituteId) VALUES (@Latitude, @Longitude, @Department_id, @Radius_In_Meters, @Search_Location, @InstituteId)";
                    await _dbConnection.ExecuteAsync(insertQuery, geoFencing);
                }
                else
                {
                    var updateQuery = "UPDATE tbl_GeoFencing SET Latitude = @Latitude, Longitude = @Longitude, Department_id = @Department_id, Radius_In_Meters = @Radius_In_Meters, Search_Location = @Search_Location, InstituteId = @InstituteId WHERE Geo_Fencing_id = @Geo_Fencing_id";
                    await _dbConnection.ExecuteAsync(updateQuery, geoFencing);
                }
            }
            return new ServiceResponse<bool>(true, "All GeoFencings processed successfully", true, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteGeoFencing(int id)
        {
            var query = "UPDATE tbl_GeoFencing SET IsDeleted = 1 WHERE Geo_Fencing_id = @Id";
            var parameters = new { Id = id };
            await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<bool>(true, "GeoFencing deleted", true, 200);
        }
    }
}
