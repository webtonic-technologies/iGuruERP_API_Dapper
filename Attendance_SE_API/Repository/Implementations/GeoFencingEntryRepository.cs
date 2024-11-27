using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.ServiceResponse;
using Attendance_SE_API.Repository.Interfaces;

namespace Attendance_SE_API.Repository.Implementations
{
    public class GeoFencingEntryRepository : IGeoFencingEntryRepository
    {
        private readonly IConfiguration _config;

        public GeoFencingEntryRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<ServiceResponse<string>> AddGeoFencingEntry(GeoFencingEntryRequest request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var query = @"INSERT INTO tblGeoFencingEntry (EmployeeID, GeoFencingDate, ClockIn, ClockOut, Reason, InstituteID)
                      VALUES (@EmployeeID, @GeoFencingDate, @ClockIn, @ClockOut, @Reason, @InstituteID)";

                // Parse date and time correctly
                var parsedClockIn = DateTime.ParseExact(request.ClockIn, "hh:mm tt", null).TimeOfDay;
                var parsedClockOut = DateTime.ParseExact(request.ClockOut, "hh:mm tt", null).TimeOfDay;

                await connection.ExecuteAsync(query, new
                {
                    request.EmployeeID,
                    GeoFencingDate = DateTime.ParseExact(request.GeoFencingDate, "dd-MM-yyyy", null),
                    ClockIn = parsedClockIn,
                    ClockOut = parsedClockOut,
                    request.Reason,
                    request.InstituteID
                });

                return new ServiceResponse<string>(true, "Geo-fencing entry added successfully", null, 200);
            }
        }

        public async Task<ServiceResponse<IEnumerable<GeoFencingEntryResponse>>> GetGeoFencingEntry(GeoFencingEntryRequest2 request)
        {
            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                var query = @"
                    SELECT e.Employee_id AS EmployeeID, 
                           CONCAT(e.First_Name, ' ', e.Last_Name) AS EmployeeName, 
                           e.mobile_number AS MobileNumber,
                           d.DepartmentName AS Department,
                           des.DesignationName AS Designation
                    FROM tbl_EmployeeProfileMaster e
                    JOIN tbl_Department d ON e.Department_id = d.Department_id
                    JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
                    WHERE e.Institute_id = @InstituteID";

                var result = await connection.QueryAsync<GeoFencingEntryResponse>(query, new
                {
                    InstituteID = request.InstituteID
                });

                return new ServiceResponse<IEnumerable<GeoFencingEntryResponse>>(
                    success: true,
                    message: "Geo-fencing entry data fetched successfully",
                    data: result,
                    statusCode: 200
                );
            }
        }

    }
}
