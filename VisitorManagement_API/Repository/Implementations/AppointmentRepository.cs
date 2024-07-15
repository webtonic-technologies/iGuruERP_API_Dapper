using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using VisitorManagement_API.DTOs.Requests;
using VisitorManagement_API.DTOs.Responses;
using VisitorManagement_API.DTOs.ServiceResponse;
using VisitorManagement_API.Models;
using VisitorManagement_API.Repository.Interfaces;

namespace VisitorManagement_API.Repository.Implementations
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDbConnection _dbConnection;

        public AppointmentRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<ServiceResponse<string>> AddUpdateAppointment(Appointment appointment)
        {
            try
            {
                if (appointment.AppointmentID == 0)
                {
                    // Insert new appointment
                    string query = @"INSERT INTO tblAppointment (Appointee, OrganizationName, MobileNo, EmailID, PurposeID, EmployeeID, CheckInTime, CheckOutTime, Description, NoOfVisitors, Status, InstituteId, ApprovalStatus)
                                     VALUES (@Appointee, @OrganizationName, @MobileNo, @EmailID, @PurposeID, @EmployeeID, @CheckInTime, @CheckOutTime, @Description, @NoOfVisitors, @Status, @InstituteId, @ApprovalStatus)";
                    int insertedValue = await _dbConnection.ExecuteAsync(query, appointment);
                    if (insertedValue > 0)
                    {
                        return new ServiceResponse<string>(true, "Appointment Added Successfully", "Success", 201);
                    }
                    return new ServiceResponse<string>(false, "Failed to Add Appointment", "Failure", 400);
                }
                else
                {
                    // Update existing appointment
                    string query = @"UPDATE tblAppointment SET Appointee = @Appointee, OrganizationName = @OrganizationName, MobileNo = @MobileNo, EmailID = @EmailID, PurposeID = @PurposeID, EmployeeID = @EmployeeID,
                                    CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, Description = @Description, NoOfVisitors = @NoOfVisitors, 
                                    Status = @Status, InstituteId = @InstituteId, ApprovalStatus = @ApprovalStatus
                                     WHERE AppointmentID = @AppointmentID";
                    int rowsAffected = await _dbConnection.ExecuteAsync(query, appointment);
                    if (rowsAffected > 0)
                    {
                        return new ServiceResponse<string>(true, "Appointment Updated Successfully", "Success", 200);
                    }
                    return new ServiceResponse<string>(false, "Failed to Update Appointment", "Failure", 400);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>(false, ex.Message, "Error", 500);
            }
        }
        public async Task<ServiceResponse<IEnumerable<AppointmentResponse>>> GetAllAppointments(GetAllAppointmentsRequest request)
        {
            try
            {
                string query = @"
        SELECT a.AppointmentID, a.Appointee, a.OrganizationName, a.MobileNo, a.EmailID, a.PurposeID, 
               p.Purpose as PurposeName, a.EmployeeID, e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeFullName,
               a.CheckInTime, a.CheckOutTime, a.Description, a.NoOfVisitors, a.Status, a.InstituteId,
               a.ApprovalStatus, s.ApprovalType as ApprovalStatusName
        FROM tblAppointment a
        JOIN tblPurposeType p ON a.PurposeID = p.PurposeID
        JOIN tbl_EmployeeProfileMaster e ON a.EmployeeID = e.Employee_id
        JOIN tblVisitorApprovalMaster s ON a.ApprovalStatus = s.ApprovalTypeID
        WHERE a.InstituteId = @InstituteId AND a.Status = 1";

                var parameters = new DynamicParameters();
                parameters.Add("InstituteId", request.InstituteId);

                // Apply date range filter if provided
                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    query += " AND a.CheckInTime >= @StartDate AND a.CheckOutTime <= @EndDate";
                    parameters.Add("StartDate", request.StartDate);
                    parameters.Add("EndDate", request.EndDate);
                }

                var appointments = await _dbConnection.QueryAsync<AppointmentResponse>(query, parameters);

                // Pagination
                var paginatedAppointments = appointments.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

                return new ServiceResponse<IEnumerable<AppointmentResponse>>(true, "Appointments Retrieved Successfully", paginatedAppointments, 200, appointments.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<AppointmentResponse>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<AppointmentResponse>> GetAppointmentById(int appointmentId)
        {
            try
            {
                string query = @"
        SELECT a.AppointmentID, a.Appointee, a.OrganizationName, a.MobileNo, a.EmailID, a.PurposeID, 
               p.Purpose as PurposeName, a.EmployeeID, 
               e.First_Name + ' ' + e.Middle_Name + ' ' + e.Last_Name AS EmployeeFullName,
               a.CheckInTime, a.CheckOutTime, a.Description, a.NoOfVisitors, a.Status, a.InstituteId,
               a.ApprovalStatus, s.ApprovalType as ApprovalStatusName
        FROM tblAppointment a
        JOIN tblPurposeType p ON a.PurposeID = p.PurposeID
        JOIN tbl_EmployeeProfileMaster e ON a.EmployeeID = e.Employee_id
        JOIN tblVisitorApprovalMaster s ON a.ApprovalStatus = s.ApprovalTypeID
        WHERE a.AppointmentID = @AppointmentID AND a.Status = 1";

                var appointment = await _dbConnection.QueryFirstOrDefaultAsync<AppointmentResponse>(query, new { AppointmentID = appointmentId });

                if (appointment != null)
                {
                    return new ServiceResponse<AppointmentResponse>(true, "Appointment Retrieved Successfully", appointment, 200);
                }

                return new ServiceResponse<AppointmentResponse>(false, "Appointment Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AppointmentResponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<bool>> UpdateAppointmentStatus(int appointmentId)
        {
            try
            {
                // Assuming there is a Status column in tblAppointment table and a value of 0 means 'soft deleted'
                string query = "UPDATE tblAppointment SET Status = 0 WHERE AppointmentID = @AppointmentID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { AppointmentID = appointmentId });

                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Appointment Soft Deleted Successfully", true, 200);
                }

                return new ServiceResponse<bool>(false, "Failed to Soft Delete Appointment", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
