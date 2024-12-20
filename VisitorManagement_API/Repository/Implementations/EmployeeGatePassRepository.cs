using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;

namespace VisitorManagement_API.Repository.Implementations
{
    public class EmployeeGatePassRepository : IEmployeeGatePassRepository
    {
        private readonly IDbConnection _dbConnection;

        public EmployeeGatePassRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateEmployeeGatePass(EmployeeGatePass employeeGatePass)
        {
            try
            {
                // Convert CheckInTime and CheckOutTime to DateTime using ParseExact with AM/PM format
                DateTime checkInDateTime = DateTime.ParseExact(employeeGatePass.CheckInTime, "dd-MM-yyyy, hh:mm tt", CultureInfo.InvariantCulture);
                DateTime checkOutDateTime = DateTime.ParseExact(employeeGatePass.CheckOutTime, "dd-MM-yyyy, hh:mm tt", CultureInfo.InvariantCulture);

                // Format DateTime to the database format (standard format)
                string formattedCheckInTime = checkInDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                string formattedCheckOutTime = checkOutDateTime.ToString("yyyy-MM-dd HH:mm:ss");

                // Format DateTime for the response (with AM/PM)
                string formattedCheckInTimeForResponse = checkInDateTime.ToString("dd-MM-yyyy, hh:mm tt");
                string formattedCheckOutTimeForResponse = checkOutDateTime.ToString("dd-MM-yyyy, hh:mm tt");

                // Assign formatted values to employeeGatePass for storage and response
                employeeGatePass.CheckInTime = formattedCheckInTimeForResponse;
                employeeGatePass.CheckOutTime = formattedCheckOutTimeForResponse;

                if (employeeGatePass.GatePassID == 0)
                {
                    // Insert new employee gate pass, excluding IsDeleted
                    string query = @"INSERT INTO tblGatePass (EmployeeID, PassNo, VisitorFor, CheckOutTime, CheckInTime, Purpose, PlanOfVisit, Remarks, StatusID, InstituteId)
                             VALUES (@EmployeeID, @PassNo, @VisitorFor, @CheckOutTime, @CheckInTime, @Purpose, @PlanOfVisit, @Remarks, @StatusID, @InstituteId)";

                    // Removing IsDeleted from the parameters
                    int insertedValue = await _dbConnection.ExecuteAsync(query, new
                    {
                        employeeGatePass.EmployeeID,
                        employeeGatePass.PassNo,
                        employeeGatePass.VisitorFor,
                        CheckInTime = formattedCheckInTime,
                        CheckOutTime = formattedCheckOutTime,
                        employeeGatePass.Purpose,
                        employeeGatePass.PlanOfVisit,
                        employeeGatePass.Remarks,
                        employeeGatePass.StatusID,
                        employeeGatePass.InstituteId
                    });

                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Employee Gate Pass Added Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Employee Gate Pass", "Failure", 400);
                }
                else
                {
                    // Update existing employee gate pass, excluding IsDeleted
                    string query = @"UPDATE tblGatePass SET EmployeeID = @EmployeeID, PassNo = @PassNo, VisitorFor = @VisitorFor, CheckOutTime = @CheckOutTime, CheckInTime = @CheckInTime, Purpose = @Purpose, PlanOfVisit = @PlanOfVisit, Remarks = @Remarks, StatusID = @StatusID, InstituteId = @InstituteId
                             WHERE GatePassID = @GatePassID";

                    // Removing IsDeleted from the parameters
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, new
                    {
                        employeeGatePass.GatePassID,
                        employeeGatePass.EmployeeID,
                        employeeGatePass.PassNo,
                        employeeGatePass.VisitorFor,
                        CheckInTime = formattedCheckInTime,
                        CheckOutTime = formattedCheckOutTime,
                        employeeGatePass.Purpose,
                        employeeGatePass.PlanOfVisit,
                        employeeGatePass.Remarks,
                        employeeGatePass.StatusID,
                        employeeGatePass.InstituteId
                    });

                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "Employee Gate Pass Updated Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Update Employee Gate Pass", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        //public async Task<ServiceResponse<IEnumerable<EmployeeGatepassResponse>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request)
        //{
        //    try
        //    {
        //        string query = @"
        //        SELECT gp.GatePassID, gp.EmployeeID, 
        //               e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeName, 
        //               e.Department_id AS Departmentid, d.DepartmentName, 
        //               e.Designation_id AS Designationid, des.DesignationName, 
        //               gp.PassNo, gp.VisitorFor, vf.VisitedForName as VisitorForName, gp.CheckOutTime, gp.CheckInTime, 
        //               gp.Purpose, gp.PlanOfVisit, gp.Remarks, gp.StatusID, a.ApprovalType AS StatusName, gp.InstituteId
        //        FROM tblGatePass gp
        //        JOIN tbl_EmployeeProfileMaster e ON gp.EmployeeID = e.Employee_id
        //        JOIN tbl_Department d ON e.Department_id = d.Department_id
        //        JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        //        JOIN tblVisitorApprovalMaster a ON gp.StatusID = a.ApprovalTypeID
        //        JOIN tblVisitedFor vf ON gp.VisitorFor = vf.VisitedFor
        //        WHERE gp.IsDeleted = 0 AND gp.InstituteId = @InstituteId";

        //        var employeeGatePasses = await _dbConnection.QueryAsync<EmployeeGatepassResponse>(query, new { request.InstituteId });

        //        // Apply filters only if they are provided
        //        if (!string.IsNullOrWhiteSpace(request.StartDate) && !string.IsNullOrWhiteSpace(request.EndDate))
        //        {
        //            DateTime startDate, endDate;

        //            // Parse the StartDate and EndDate strings to DateTime
        //            if (DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) &&
        //                DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
        //            {
        //                // Use DateTime comparison directly without parsing again
        //                employeeGatePasses = employeeGatePasses.Where(gp => gp.CheckInTime >= startDate && gp.CheckInTime <= endDate);
        //            }
        //        }

        //        if (request.EmployeeId > 0)
        //        {
        //            employeeGatePasses = employeeGatePasses.Where(gp => gp.EmployeeID == request.EmployeeId);
        //        }

        //        if (!string.IsNullOrWhiteSpace(request.SearchText))
        //        {
        //            employeeGatePasses = employeeGatePasses.Where(gp => gp.EmployeeName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
        //                                                                gp.PassNo.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
        //                                                                gp.Purpose.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase));
        //        }

        //        // Pagination
        //        var paginatedEmployeeGatePasses = employeeGatePasses.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

        //        return new ServiceResponse<IEnumerable<EmployeeGatepassResponse>>(true, "Employee Gate Passes Retrieved Successfully", paginatedEmployeeGatePasses, 200, employeeGatePasses.Count());
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<IEnumerable<EmployeeGatepassResponse>>(false, ex.Message, null, 500);
        //    }
        //}

        public async Task<ServiceResponse<IEnumerable<EmployeeGatepassResponse>>> GetAllEmployeeGatePass(GetAllEmployeeGatePassRequest request)
        {
            try
            {
                // SQL query to retrieve employee gate passes from the database
                string query = @"
        SELECT gp.GatePassID, gp.EmployeeID, 
               e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeName, 
               e.Department_id AS Departmentid, d.DepartmentName, 
               e.Designation_id AS Designationid, des.DesignationName, 
               gp.PassNo, gp.VisitorFor, vf.VisitedForName as VisitorForName, gp.CheckOutTime, gp.CheckInTime, 
               gp.Purpose, gp.PlanOfVisit, gp.Remarks, gp.StatusID, a.ApprovalType AS StatusName, gp.InstituteId
        FROM tblGatePass gp
        JOIN tbl_EmployeeProfileMaster e ON gp.EmployeeID = e.Employee_id
        JOIN tbl_Department d ON e.Department_id = d.Department_id
        JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        JOIN tblVisitorApprovalMaster a ON gp.StatusID = a.ApprovalTypeID
        JOIN tblVisitedFor vf ON gp.VisitorFor = vf.VisitedFor
        WHERE gp.IsDeleted = 0 AND gp.InstituteId = @InstituteId";

                // Fetch data from the database
                var employeeGatePasses = await _dbConnection.QueryAsync<EmployeeGatepassResponse>(query, new { request.InstituteId });

                // Apply filters if StartDate and EndDate are provided
                if (!string.IsNullOrWhiteSpace(request.StartDate) && !string.IsNullOrWhiteSpace(request.EndDate))
                {
                    DateTime startDate, endDate;

                    // Parse the StartDate and EndDate strings to DateTime
                    if (DateTime.TryParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate) &&
                        DateTime.TryParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                    {
                        // Filter employeeGatePasses based on CheckInTime (which is already DateTime)
                        employeeGatePasses = employeeGatePasses.Where(gp => gp.CheckInTime >= startDate && gp.CheckInTime <= endDate);
                    }
                }

                // Apply employee ID filter if provided
                if (request.EmployeeId > 0)
                {
                    employeeGatePasses = employeeGatePasses.Where(gp => gp.EmployeeID == request.EmployeeId);
                }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    employeeGatePasses = employeeGatePasses.Where(gp => gp.EmployeeName.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                                                         gp.PassNo.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                                                         gp.Purpose.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase));
                }

                // Pagination logic: Skip the records that don't belong to the current page and take the required number of records
                var paginatedEmployeeGatePasses = employeeGatePasses.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

                // Return the response with paginated employee gate passes
                return new ServiceResponse<IEnumerable<EmployeeGatepassResponse>>(true, "Employee Gate Passes Retrieved Successfully", paginatedEmployeeGatePasses, 200, employeeGatePasses.Count());
            }
            catch (Exception ex)
            {
                // In case of an exception, return an error response
                return new ServiceResponse<IEnumerable<EmployeeGatepassResponse>>(false, ex.Message, null, 500);
            }
        }




        public async Task<ServiceResponse<EmployeeGatepassResponse>> GetEmployeeGatePassById(int gatePassId) 
        {
            try
            {
                string query = @"
        SELECT gp.GatePassID, gp.EmployeeID, 
               e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeName, 
               e.Department_id AS Departmentid, d.DepartmentName, 
               e.Designation_id AS Designationid, des.DesignationName, 
               gp.PassNo, gp.VisitorFor, vf.VisitedForName as VisitorForName, gp.CheckOutTime, gp.CheckInTime, 
               gp.Purpose, gp.PlanOfVisit, gp.Remarks, gp.StatusID, a.ApprovalType AS StatusName, gp.InstituteId
        FROM tblGatePass gp
        JOIN tbl_EmployeeProfileMaster e ON gp.EmployeeID = e.Employee_id
        JOIN tbl_Department d ON e.Department_id = d.Department_id
        JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
        JOIN tblVisitorApprovalMaster a ON gp.StatusID = a.ApprovalTypeID
        JOIN tblVisitedFor vf ON gp.VisitorFor = vf.VisitedFor
        WHERE gp.GatePassID = @GatePassID AND gp.IsDeleted = 0";

                var employeeGatePass = await _dbConnection.QueryFirstOrDefaultAsync<EmployeeGatepassResponse>(query, new { GatePassID = gatePassId });

                if (employeeGatePass != null)
                {
                    return new ServiceResponse<EmployeeGatepassResponse>(true, "Employee Gate Pass Retrieved Successfully", employeeGatePass, 200);
                }

                return new ServiceResponse<EmployeeGatepassResponse>(false, "Employee Gate Pass Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeGatepassResponse>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateEmployeeGatePassStatus(int gatePassId)
        {
            try
            {
                // Assuming there is an IsDeleted column in tblGatePass table and a value of 1 means 'soft deleted'
                string query = "UPDATE tblGatePass SET IsDeleted = 1 WHERE GatePassID = @GatePassID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { GatePassID = gatePassId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Employee Gate Pass Status Updated Successfully (Soft Deleted)", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to Update Employee Gate Pass Status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
        public async Task<ServiceResponse<List<Visitedfor>>> GetAllVisitedForReason()
        {
            try
            {
                string query = "select * from tblVisitedFor";
                var rowsAffected = await _dbConnection.QueryAsync<Visitedfor>(query);

                if (rowsAffected.Any())
                {
                    return new ServiceResponse<List<Visitedfor>>(true, "Records found", rowsAffected.AsList(), 200);
                }

                return new ServiceResponse<List<Visitedfor>>(false, "Failed to Update Employee Gate Pass Status", [], 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<Visitedfor>>(false, ex.Message, [], 500);
            }
        }

        public async Task<List<GetVisitorForDDLResponse>> GetVisitorForDDL()
        {
            string query = "SELECT VisitedFor, VisitedForName FROM tblVisitedFor";
            var result = await _dbConnection.QueryAsync<GetVisitorForDDLResponse>(query);
            return result.AsList();
        }

        public async Task<GetGatePassSlipResponse> GetGatePassSlip(int gatePassID, int instituteID)
        {
            // Query to fetch institute details
            string instituteQuery = @"
                SELECT 
                    i.Institute_name AS InstituteName, 
                    CONCAT(ia.house, ', ', ia.Locality, ', ', ia.Landmark, ', ', ia.pincode) AS Address, 
                    ia.Mobile_number AS Mobile, 
                    ia.Email
                FROM tbl_InstituteDetails i
                JOIN tbl_InstituteAddress ia ON i.Institute_id = ia.Institute_id
                WHERE i.Institute_id = @InstituteID";

            var instituteInfo = await _dbConnection.QueryFirstOrDefaultAsync<GatePassInstituteInfo>(instituteQuery, new { InstituteID = instituteID });

            if (instituteInfo == null)
            {
                return null;  // Institute not found
            }

            // Query to fetch visitor details related to the GatePass
            string visitorQuery = @"
                SELECT 
                    gp.PassNo,
                    CONCAT(e.First_Name, ' ', e.Middle_Name, ' ', e.Last_Name) AS EmployeeName,
                    e.Employee_code_id AS EmployeeCode,
                    e.mobile_number AS MobileNumber,
                    vf.VisitedForName AS VisitorFor,
                    gp.PlanOfVisit,
                    gp.Purpose,
                    gp.Remarks,
                    FORMAT(gp.CheckInTime, 'dd-MM-yyyy, hh:mm tt') AS CheckInTime,
                    FORMAT(gp.CheckOutTime, 'dd-MM-yyyy, hh:mm tt') AS CheckOutTime
                FROM tblGatePass gp
                LEFT JOIN tbl_EmployeeProfileMaster e ON gp.EmployeeID = e.Employee_id
                LEFT JOIN tblVisitedFor vf ON gp.VisitorFor = vf.VisitedFor
                WHERE gp.GatePassID = @GatePassID AND gp.InstituteID = @InstituteID";

            var visitorSlip = await _dbConnection.QueryFirstOrDefaultAsync<GatePassVisitorSlip>(visitorQuery, new { GatePassID = gatePassID, InstituteID = instituteID });

            if (visitorSlip == null)
            {
                return null;  // GatePass not found
            }

            // Combine institute and visitor data into response
            return new GetGatePassSlipResponse
            {
                InstituteInfo = instituteInfo,
                VisitorSlip = visitorSlip
            };
        }



        public async Task<IEnumerable<GetEmployeeGatePassExportResponse>> GetEmployeeGatePassExport(GetEmployeeGatePassExportRequest request)
        {
            string query = @"
            SELECT gp.GatePassID, gp.EmployeeID, 
                e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeName, 
                e.Department_id AS Departmentid, d.DepartmentName, 
                e.Designation_id AS Designationid, des.DesignationName, 
                gp.PassNo, gp.VisitorFor, vf.VisitedForName as VisitorForName, 
                gp.CheckOutTime, gp.CheckInTime, gp.Purpose, gp.PlanOfVisit, 
                gp.Remarks, gp.StatusID, a.ApprovalType AS StatusName, gp.InstituteId
            FROM tblGatePass gp
            JOIN tbl_EmployeeProfileMaster e ON gp.EmployeeID = e.Employee_id
            JOIN tbl_Department d ON e.Department_id = d.Department_id
            JOIN tbl_Designation des ON e.Designation_id = des.Designation_id
            JOIN tblVisitorApprovalMaster a ON gp.StatusID = a.ApprovalTypeID
            JOIN tblVisitedFor vf ON gp.VisitorFor = vf.VisitedFor
            WHERE gp.IsDeleted = 0 AND gp.InstituteId = @InstituteId";

            // Prepare the parameters
            var parameters = new
            {
                InstituteId = request.InstituteId,
                EmployeeId = request.EmployeeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                SearchText = request.SearchText
            };

            // Execute the query and fetch results
            var GatePass = await _dbConnection.QueryAsync<GetEmployeeGatePassExportResponse>(query, parameters);
            return GatePass;
        }

    }
}
