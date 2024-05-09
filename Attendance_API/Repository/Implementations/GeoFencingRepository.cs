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
            var query = "SELECT g.*, d.DepartmentName as Department_Name FROM tbl_GeoFencing g JOIN tbl_Department d ON g.Department_id = d.Department_id WHERE Geo_Fencing_id = @Id";
            var parameters = new { Id = id };
            var geoFencing = await _dbConnection.QueryFirstOrDefaultAsync<GeoFencingResponseDTO>(query, parameters);
            return new ServiceResponse<GeoFencingResponseDTO>(true, "GeoFencing found", geoFencing, 200);
        }

        public async Task<ServiceResponse<IEnumerable<GeoFencingResponseDTO>>> GetAllGeoFencings()
        {
            var query = "SELECT g.*, d.DepartmentName as Department_Name FROM tbl_GeoFencing g JOIN tbl_Department d ON g.Department_id = d.Department_id";
            var geoFencings = await _dbConnection.QueryAsync<GeoFencingResponseDTO>(query);
            return new ServiceResponse<IEnumerable<GeoFencingResponseDTO>>(true, "All GeoFencings found", geoFencings, 200);
        }

        public async Task<ServiceResponse<bool>> AddGeoFencing(GeoFencingDTO geoFencing)
        {
            var checkQuery = "SELECT COUNT(1) FROM tbl_GeoFencing WHERE Latitude = @Latitude AND Longitude = @Longitude AND Department_id = @Department_id AND Radius_In_Meters = @Radius_In_Meters AND Search_Location = @Search_Location";
            var checkParameters = new { Latitude = geoFencing.Latitude, Longitude = geoFencing.Longitude, Department_id = geoFencing.Department_id, Radius_In_Meters = geoFencing.Radius_In_Meters, Search_Location = geoFencing.Search_Location };
            var recordCount = await _dbConnection.ExecuteScalarAsync<int>(checkQuery, checkParameters);
            if (recordCount > 0)
            {
                return new ServiceResponse<bool>(false, "GeoFencing already exists", false, 409);
            }
            var insertQuery = "INSERT INTO tbl_GeoFencing (Latitude, Longitude, Department_id, Radius_In_Meters, Search_Location) VALUES (@Latitude, @Longitude, @Department_id, @Radius_In_Meters, @Search_Location)";
            await _dbConnection.ExecuteAsync(insertQuery, geoFencing);
            return new ServiceResponse<bool>(true, "GeoFencing added", true, 200);
        }

        public async Task<ServiceResponse<bool>> UpdateGeoFencing(GeoFencingDTO geoFencing)
        {
            var checkQuery = "SELECT COUNT(1) FROM tbl_GeoFencing WHERE Geo_Fencing_id = @Geo_Fencing_id";
            var checkParameters = new { Geo_Fencing_id = geoFencing.Geo_Fencing_id };
            var recordCount = await _dbConnection.ExecuteScalarAsync<int>(checkQuery, checkParameters);
            if (recordCount == 0)
            {
                return new ServiceResponse<bool>(false, "GeoFencing not found", false, 404);
            }
            var updateQuery = "UPDATE tbl_GeoFencing SET Latitude = @Latitude, Longitude = @Longitude, Department_id = @Department_id, Radius_In_Meters = @Radius_In_Meters, Search_Location = @Search_Location WHERE Geo_Fencing_id = @Geo_Fencing_id";
            await _dbConnection.ExecuteAsync(updateQuery, geoFencing);
            return new ServiceResponse<bool>(true, "GeoFencing updated", true, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteGeoFencing(int id)
        {
            var query = "DELETE FROM tbl_GeoFencing WHERE Geo_Fencing_id = @Id";
            var parameters = new { Id = id };
            await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<bool>(true, "GeoFencing deleted", true, 200);
        }
    }
}

