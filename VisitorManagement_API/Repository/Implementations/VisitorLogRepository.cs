using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;

namespace VisitorManagement_API.Repository.Implementations
{
    public class VisitorLogRepository : IVisitorLogRepository
    {
        private readonly IDbConnection _dbConnection;

        public VisitorLogRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateVisitorLog(VisitorLog visitorLog)
        {
            try
            {
                if (visitorLog.VisitorID == 0)
                {
                    // Insert new visitor log
                    string query = @"INSERT INTO tblVisitorMaster (VisitorCodeID, VisitorName, Photo, SourceID, PurposeID, MobileNo, EmailID, Address, OrganizationName, EmployeeID, NoOfVisitor, AccompaniedBy, CheckInTime, CheckOutTime, Remarks, IDProofDocumentID, Information, Document, ApprovalTypeID, Status)
                                     VALUES (@VisitorCodeID, @VisitorName, @Photo, @SourceID, @PurposeID, @MobileNo, @EmailID, @Address, @OrganizationName, @EmployeeID, @NoOfVisitor, @AccompaniedBy, @CheckInTime, @CheckOutTime, @Remarks, @IDProofDocumentID, @Information, @Document, @ApprovalTypeID, @Status)";
                    int insertedValue = await _dbConnection.ExecuteAsync(query, visitorLog);
                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Visitor Log Added Successfully", "Success", 201);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Visitor Log", "Failure", 400);
                }
                else
                {
                    // Update existing visitor log
                    string query = @"UPDATE tblVisitorMaster SET VisitorCodeID = @VisitorCodeID, VisitorName = @VisitorName, Photo = @Photo, SourceID = @SourceID, PurposeID = @PurposeID, MobileNo = @MobileNo, EmailID = @EmailID, Address = @Address, OrganizationName = @OrganizationName, EmployeeID = @EmployeeID, NoOfVisitor = @NoOfVisitor, AccompaniedBy = @AccompaniedBy, CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, Remarks = @Remarks, IDProofDocumentID = @IDProofDocumentID, Information = @Information, Document = @Document, ApprovalTypeID = @ApprovalTypeID, Status = @Status WHERE VisitorID = @VisitorID";
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, visitorLog);
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "Visitor Log Updated Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Update Visitor Log", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }

        public async Task<ServiceResponse<IEnumerable<VisitorLog>>> GetAllVisitorLogs(GetAllVisitorLogsRequest request)
        {
            try
            {
                string query = "SELECT * FROM tblVisitorMaster";
                var visitorLogs = await _dbConnection.QueryAsync<VisitorLog>(query);
                var paginatedVisitorLogs = visitorLogs.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                return new ServiceResponse<IEnumerable<VisitorLog>>(true, "Visitor Logs Retrieved Successfully", paginatedVisitorLogs, 200, visitorLogs.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<VisitorLog>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<VisitorLog>> GetVisitorLogById(int visitorId)
        {
            try
            {
                string query = "SELECT * FROM tblVisitorMaster WHERE VisitorID = @VisitorID";
                var visitorLog = await _dbConnection.QueryFirstOrDefaultAsync<VisitorLog>(query, new { VisitorID = visitorId });
                if (visitorLog != null)
                {
                    return new ServiceResponse<VisitorLog>(true, "Visitor Log Retrieved Successfully", visitorLog, 200);
                }
                return new ServiceResponse<VisitorLog>(false, "Visitor Log Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<VisitorLog>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateVisitorLogStatus(int visitorId)
        {
            try
            {
                // Assuming there is a Status column in tblVisitorMaster table
                string query = "UPDATE tblVisitorMaster SET Status = CASE WHEN Status = 1 THEN 0 ELSE 1 END WHERE VisitorID = @VisitorID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { VisitorID = visitorId });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Visitor Log Status Updated Successfully", true, 200);
                }
                return new ServiceResponse<bool>(false, "Failed to Update Visitor Log Status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
