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
    public class EmployeeShiftRepository : IEmployeeShiftRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeShiftRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<string>> AddUpdateShift(ShiftRequest request)
        {
            if (request.Designations == null || !request.Designations.Any())
            {
                return new ServiceResponse<string>(false, "Designations list is required.", null, 400);
            }

            string query;

            if (request.ShiftID > 0) // Update existing shift
            {
                // Remove previously mapped designations
                string deleteQuery = @"DELETE FROM tblShiftMasterMapping WHERE ShiftID = @ShiftID";
                await _connection.ExecuteAsync(deleteQuery, new { ShiftID = request.ShiftID });

                query = @"UPDATE tblShiftMaster 
                  SET ClockIn = @ClockIn, ClockOut = @ClockOut, LateComing = @LateComing, InstituteID = @InstituteID 
                  WHERE ShiftID = @ShiftID";
                await _connection.ExecuteAsync(query, new
                {
                    ShiftID = request.ShiftID,
                    ClockIn = request.ClockIn, // No conversion needed; TimeSpan is accepted directly
                    ClockOut = request.ClockOut, // No conversion needed; TimeSpan is accepted directly
                    LateComing = request.LateComing, // No conversion needed; TimeSpan is accepted directly
                    InstituteID = request.InstituteID // Include InstituteID in the update
                });
            }
            else // Add new shift
            {
                query = @"INSERT INTO tblShiftMaster (ClockIn, ClockOut, LateComing, InstituteID) 
                  VALUES (@ClockIn, @ClockOut, @LateComing, @InstituteID);
                  SELECT CAST(SCOPE_IDENTITY() as int)";
                request.ShiftID = await _connection.QuerySingleAsync<int>(query, new
                {
                    ClockIn = request.ClockIn, // No conversion needed; TimeSpan is accepted directly
                    ClockOut = request.ClockOut, // No conversion needed; TimeSpan is accepted directly
                    LateComing = request.LateComing, // No conversion needed; TimeSpan is accepted directly
                    InstituteID = request.InstituteID // Include InstituteID in the insert
                });
            }

            // Logic for handling designations in tblShiftMasterMapping
            foreach (var designation in request.Designations)
            {
                string mappingQuery = @"INSERT INTO tblShiftMasterMapping (ShiftID, DesignationID) 
                                VALUES (@ShiftID, @DesignationID)";
                await _connection.ExecuteAsync(mappingQuery, new
                {
                    ShiftID = request.ShiftID,
                    DesignationID = designation.DesignationID
                });
            }

            return new ServiceResponse<string>(true, "Shift processed successfully.", null, 200);
        }

        //public async Task<ServiceResponse<List<ShiftResponse>>> GetAllShifts(GetAllShiftsRequest request)
        //{
        //    // Define the query with pagination and institute filter
        //    string query = @"
        //SELECT sm.ShiftID, 
        //       sm.ClockIn, 
        //       sm.ClockOut, 
        //       sm.LateComing,
        //       d.DesignationName 
        //FROM tblShiftMaster sm
        //LEFT JOIN tblShiftMasterMapping smm ON sm.ShiftID = smm.ShiftID
        //LEFT JOIN tbl_Designation d ON smm.DesignationID = d.Designation_id
        //WHERE sm.InstituteID = @InstituteID
        //ORDER BY sm.ShiftID
        //OFFSET (@PageNumber - 1) * @PageSize ROWS 
        //FETCH NEXT @PageSize ROWS ONLY;";

        //    var parameters = new
        //    {
        //        InstituteID = request.InstituteID,
        //        PageNumber = request.PageNumber,
        //        PageSize = request.PageSize
        //    };

        //    var shifts = await _connection.QueryAsync<dynamic>(query, parameters);

        //    // Create a response structure
        //    var response = shifts
        //        .GroupBy(s => new { s.ShiftID, s.ClockIn, s.ClockOut, s.LateComing })
        //        .Select(g => new ShiftResponse
        //        {
        //            ShiftID = g.Key.ShiftID,
        //            ClockIn = g.Key.ClockIn,
        //            ClockOut = g.Key.ClockOut,
        //            LateComing = g.Key.LateComing,
        //            Designations = g.Select(d => new DesignationResponse
        //            {
        //                DesignationName = d.DesignationName
        //            }).ToList()
        //        }).ToList();

        //    return new ServiceResponse<List<ShiftResponse>>(true, "Shifts retrieved successfully.", response, 200);
        //}


        public async Task<ServiceResponse<List<ShiftResponse>>> GetAllShifts(GetAllShiftsRequest request)
        {
            try
            {
                // Define the query with pagination and institute filter
                string query = @"
SELECT sm.ShiftID, 
       CONVERT(varchar, sm.ClockIn, 100) AS ClockIn,  -- Converted to varchar in hh:mm tt format
       CONVERT(varchar, sm.ClockOut, 100) AS ClockOut,  -- Converted to varchar
       CONVERT(varchar, sm.LateComing, 100) AS LateComing,  -- Converted to varchar
       d.DesignationName 
FROM tblShiftMaster sm
LEFT JOIN tblShiftMasterMapping smm ON sm.ShiftID = smm.ShiftID
LEFT JOIN tbl_Designation d ON smm.DesignationID = d.Designation_id
WHERE sm.InstituteID = @InstituteID AND sm.IsActive = 1
ORDER BY sm.ShiftID
OFFSET (@PageNumber - 1) * @PageSize ROWS 
FETCH NEXT @PageSize ROWS ONLY;";

                var parameters = new
                {
                    InstituteID = request.InstituteID,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                var shifts = await _connection.QueryAsync<dynamic>(query, parameters);

                // Group and format data as needed
                var response = shifts
                    .GroupBy(s => new { s.ShiftID, s.ClockIn, s.ClockOut, s.LateComing })
                    .Select(g => new ShiftResponse
                    {
                        ShiftID = g.Key.ShiftID,
                        ClockIn = g.Key.ClockIn,  // Directly assign the formatted string from SQL query
                        ClockOut = g.Key.ClockOut,  // Directly assign the formatted string from SQL query
                        LateComing = g.Key.LateComing,  // Directly assign the formatted string from SQL query
                        Designations = g.Select(d => new DesignationResponse
                        {
                            DesignationName = d.DesignationName
                        }).ToList()
                    }).ToList();

                return new ServiceResponse<List<ShiftResponse>>(true, "Shifts retrieved successfully.", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ShiftResponse>>(false, ex.Message, null, 500);
            }
        }



        //public async Task<ServiceResponse<ShiftResponse>> GetShiftById(int shiftID)
        //{
        //    // Define the main query to get shift details
        //    string shiftQuery = "SELECT * FROM tblShiftMaster WHERE ShiftID = @ShiftID";
        //    var shift = await _connection.QueryFirstOrDefaultAsync<ShiftResponse>(shiftQuery, new { ShiftID = shiftID });

        //    if (shift != null)
        //    {
        //        // Define a query to get designations related to the shift
        //        string designationQuery = @"
        //    SELECT d.DesignationName 
        //    FROM tblShiftMasterMapping smm
        //    INNER JOIN tbl_Designation d ON smm.DesignationID = d.Designation_id
        //    WHERE smm.ShiftID = @ShiftID";

        //        var designationNames = await _connection.QueryAsync<DesignationResponse>(designationQuery, new { ShiftID = shiftID });

        //        // Populate the designations in the shift response
        //        shift.Designations = designationNames.ToList();

        //        // Create the response object for a single shift
        //        return new ServiceResponse<ShiftResponse>(
        //            success: true,
        //            message: "Shift found.",
        //            data: shift, // Return the single shift object
        //            statusCode: 200,
        //            totalCount: null // This can be set to null as it's not a list
        //        );
        //    }

        //    // If no shift found
        //    return new ServiceResponse<ShiftResponse>(
        //        success: false,
        //        message: "Shift not found.",
        //        data: null,
        //        statusCode: 404,
        //        totalCount: null // This can also be set to null
        //    );
        //}

        public async Task<ServiceResponse<ShiftResponse>> GetShiftById(int shiftID)
        {
            try
            {
                // Define the main query to get shift details
                string shiftQuery = "SELECT ShiftID, CONVERT(varchar, ClockIn, 100) AS ClockIn, CONVERT(varchar, ClockOut, 100) AS ClockOut, CONVERT(varchar, LateComing, 100) AS LateComing, IsActive, InstituteID FROM tblShiftMaster WHERE ShiftID = @ShiftID  AND IsActive = 1";

                // Fetch the shift details
                var shift = await _connection.QueryFirstOrDefaultAsync<ShiftResponse>(shiftQuery, new { ShiftID = shiftID });

                if (shift != null)
                {
                    // Define a query to get designations related to the shift
                    string designationQuery = @"
            SELECT d.DesignationName 
            FROM tblShiftMasterMapping smm
            INNER JOIN tbl_Designation d ON smm.DesignationID = d.Designation_id
            WHERE smm.ShiftID = @ShiftID";

                    // Fetch designations for the shift
                    var designationNames = await _connection.QueryAsync<DesignationResponse>(designationQuery, new { ShiftID = shiftID });

                    // Map the designations to the shift response
                    shift.Designations = designationNames.ToList();

                    // Create the response object for a single shift
                    return new ServiceResponse<ShiftResponse>(
                        success: true,
                        message: "Shift found.",
                        data: shift,
                        statusCode: 200,
                        totalCount: null // Not applicable here since it's a single shift
                    );
                }

                // Return not found response if no shift was found
                return new ServiceResponse<ShiftResponse>(
                    success: false,
                    message: "Shift not found.",
                    data: null,
                    statusCode: 404,
                    totalCount: null
                );
            }
            catch (Exception ex)
            {
                // Log or handle the error
                return new ServiceResponse<ShiftResponse>(
                    success: false,
                    message: $"Error occurred: {ex.Message}",
                    data: null,
                    statusCode: 500,
                    totalCount: null
                );
            }
        }



        public async Task<ServiceResponse<bool>> DeleteShift(int shiftID)
        {
            // Update the IsActive column to 0 to mark the shift as inactive (soft delete)
            string query = "UPDATE tblShiftMaster SET IsActive = 0 WHERE ShiftID = @ShiftID";
            var rowsAffected = await _connection.ExecuteAsync(query, new { ShiftID = shiftID });

            // Return the result of the operation
            if (rowsAffected > 0)
            {
                return new ServiceResponse<bool>(true, "Shift deleted successfully (soft delete).", true, 200);
            }
            return new ServiceResponse<bool>(false, "Shift not found or already inactive.", false, 404);
        }

    }
}
