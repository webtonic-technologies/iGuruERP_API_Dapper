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
                    string query = @"INSERT INTO tblAppointment (Appointee, OrganizationName, MobileNo, EmailID, PurposeID, EmployeeID, CheckInTime, CheckOutTime, Description, NoOfVisitors, Status)
                                     VALUES (@Appointee, @OrganizationName, @MobileNo, @EmailID, @PurposeID, @EmployeeID, @CheckInTime, @CheckOutTime, @Description, @NoOfVisitors, @Status)";
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
                    string query = @"UPDATE tblAppointment SET Appointee = @Appointee, OrganizationName = @OrganizationName, MobileNo = @MobileNo, EmailID = @EmailID, PurposeID = @PurposeID, EmployeeID = @EmployeeID, CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, Description = @Description, NoOfVisitors = @NoOfVisitors, Status = @Status
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

        public async Task<ServiceResponse<IEnumerable<Appointment>>> GetAllAppointments(GetAllAppointmentsRequest request)
        {
            try
            {
                string query = "SELECT * FROM tblAppointment";
                var appointments = await _dbConnection.QueryAsync<Appointment>(query);
                var paginatedAppointments = appointments.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
                return new ServiceResponse<IEnumerable<Appointment>>(true, "Appointments Retrieved Successfully", paginatedAppointments, 200, appointments.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<Appointment>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<Appointment>> GetAppointmentById(int appointmentId)
        {
            try
            {
                string query = "SELECT * FROM tblAppointment WHERE AppointmentID = @AppointmentID";
                var appointment = await _dbConnection.QueryFirstOrDefaultAsync<Appointment>(query, new { AppointmentID = appointmentId });
                if (appointment != null)
                {
                    return new ServiceResponse<Appointment>(true, "Appointment Retrieved Successfully", appointment, 200);
                }
                return new ServiceResponse<Appointment>(false, "Appointment Not Found", null, 404);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<Appointment>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAppointmentStatus(int appointmentId)
        {
            try
            {
                // Assuming there is a Status column in tblAppointment table
                string query = "UPDATE tblAppointment SET Status = CASE WHEN Status = 1 THEN 0 ELSE 1 END WHERE AppointmentID = @AppointmentID";
                int rowsAffected = await _dbConnection.ExecuteAsync(query, new { AppointmentID = appointmentId });
                if (rowsAffected > 0)
                {
                    return new ServiceResponse<bool>(true, "Appointment Status Updated Successfully", true, 200);
                }
                return new ServiceResponse<bool>(false, "Failed to Update Appointment Status", false, 400);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>(false, ex.Message, false, 500);
            }
        }
    }
}
