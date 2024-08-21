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
            if (geoFencings == null || geoFencings.Count == 0)
            {
                return new ServiceResponse<bool>(false, "No GeoFencings provided", false, 400);
            }

            var valuesList = new List<string>();
            var parameters = new DynamicParameters();

            for (int i = 0; i < geoFencings.Count; i++)
            {
                var geoFencing = geoFencings[i];
                valuesList.Add($"(@Geo_Fencing_id_{i}, @Latitude_{i}, @Longitude_{i}, @Department_id_{i}, @Radius_In_Meters_{i}, @Search_Location_{i}, @InstituteId_{i})");

                parameters.Add($"@Geo_Fencing_id_{i}", geoFencing.Geo_Fencing_id);
                parameters.Add($"@Latitude_{i}", geoFencing.Latitude);
                parameters.Add($"@Longitude_{i}", geoFencing.Longitude);
                parameters.Add($"@Department_id_{i}", geoFencing.Department_id);
                parameters.Add($"@Radius_In_Meters_{i}", geoFencing.Radius_In_Meters);  
                parameters.Add($"@Search_Location_{i}", geoFencing.Search_Location);
                parameters.Add($"@InstituteId_{i}", geoFencing.InstituteId);
            }

            var mergeQuery = $@"
        MERGE INTO tbl_GeoFencing AS target
        USING (VALUES
            {string.Join(", ", valuesList)}
        ) AS source (Geo_Fencing_id, Latitude, Longitude, Department_id, Radius_In_Meters, Search_Location, InstituteId)
        ON target.Geo_Fencing_id = source.Geo_Fencing_id
        WHEN MATCHED THEN
            UPDATE SET Latitude = source.Latitude,
                       Longitude = source.Longitude,
                       Department_id = source.Department_id,
                       Radius_In_Meters = source.Radius_In_Meters,
                       Search_Location = source.Search_Location,
                       InstituteId = source.InstituteId
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (Geo_Fencing_id, Latitude, Longitude, Department_id, Radius_In_Meters, Search_Location, InstituteId)
            VALUES (source.Geo_Fencing_id, source.Latitude, source.Longitude, source.Department_id, source.Radius_In_Meters, source.Search_Location, source.InstituteId);
    ";

            await _dbConnection.ExecuteAsync(mergeQuery, parameters);

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
