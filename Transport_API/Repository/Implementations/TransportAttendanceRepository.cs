using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;
using Transport_API.DTOs.Response;

namespace Transport_API.Repository.Implementations
{
    public class TransportAttendanceRepository : ITransportAttendanceRepository
    {
        private readonly IDbConnection _dbConnection;

        public TransportAttendanceRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateTransportAttendance(TransportAttendanceRequest request)
        {
            string sql;
            if (request.TAID == 0) // Insert operation
            {
                sql = @"INSERT INTO tblTransportAttendance (AttendanceDate, RoutePlanID, TransportAttendanceTypeID, AttendanceStatus, StudentID, Remarks, InstituteID) 
                VALUES (CONVERT(date, @AttendanceDate, 103), @RoutePlanID, @TransportAttendanceTypeID, @AttendanceStatus, @StudentID, @Remarks, @InstituteID)";
            }
            else // Update operation
            {
                sql = @"UPDATE tblTransportAttendance 
                SET AttendanceDate = CONVERT(date, @AttendanceDate, 103),
                    RoutePlanID = @RoutePlanID,
                    TransportAttendanceTypeID = @TransportAttendanceTypeID,
                    AttendanceStatus = @AttendanceStatus,
                    StudentID = @StudentID,
                    Remarks = @Remarks,
                    InstituteID = @InstituteID
                WHERE TAID = @TAID";
            }

            var result = await _dbConnection.ExecuteAsync(sql, request);
            if (result > 0)
            {
                return new ServiceResponse<string>(true, "Operation Successful", "Attendance record added/updated successfully", StatusCodes.Status200OK);
            }
            return new ServiceResponse<string>(false, "Operation Failed", "Error adding/updating attendance", StatusCodes.Status400BadRequest);
        }

        public async Task<ServiceResponse<IEnumerable<TransportAttendanceResponse>>> GetAllTransportAttendance(GetTransportAttendanceRequest request)
        {
            // SQL to fetch the total number of students for the given criteria
            string countSql = @"
                            SELECT COUNT(*)
                            FROM tbl_StudentMaster s
                            JOIN tbl_Class c ON s.class_id = c.class_id
                            JOIN tbl_Section sec ON s.section_id = sec.section_id
                            LEFT JOIN tblTransportAttendance ta ON ta.StudentID = s.student_id
                                AND ta.RoutePlanID = @RouteID
                                AND ta.TransportAttendanceTypeID = @AttendanceTypeID
                                AND ta.AttendanceDate = CONVERT(DATE, @AttendanceDate, 103)
                                AND ta.InstituteID = @InstituteID";

                                    // SQL to fetch student attendance details
                                    string sql = @"
                            SELECT 
                                s.student_id AS StudentID,
                                CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName,
                                s.Admission_Number AS AdmissionNo,
                                CONCAT(c.class_name, ' - ', sec.section_name) AS ClassSection,
                                ISNULL(ta.AttendanceStatus, '-') AS AttendanceStatus,
                                ISNULL(ta.Remarks, 'No Remarks') AS Remarks
                            FROM 
                                tbl_StudentMaster s
                            JOIN 
                                tbl_Class c ON s.class_id = c.class_id
                            JOIN 
                                tbl_Section sec ON s.section_id = sec.section_id
                            LEFT JOIN 
                                tblTransportAttendance ta ON ta.StudentID = s.student_id
                                AND ta.RoutePlanID = @RouteID
                                AND ta.TransportAttendanceTypeID = @AttendanceTypeID
                                AND ta.AttendanceDate = CONVERT(DATE, @AttendanceDate, 103)
                                AND ta.InstituteID = @InstituteID
                            ORDER BY 
                                s.student_id
                            OFFSET @Offset ROWS 
                            FETCH NEXT @PageSize ROWS ONLY;";

            // Get total student count
            int totalCount = await _dbConnection.ExecuteScalarAsync<int>(countSql, new
            {
                AttendanceDate = request.AttendanceDate,
                RouteID = request.RouteID,
                AttendanceTypeID = request.AttendanceTypeID,
                InstituteID = request.InstituteID
            });

            // Get the student attendance records
            var attendanceRecords = await _dbConnection.QueryAsync<TransportAttendanceResponse>(sql, new
            {
                AttendanceDate = request.AttendanceDate,
                RouteID = request.RouteID,
                AttendanceTypeID = request.AttendanceTypeID,
                InstituteID = request.InstituteID,
                Offset = (request.pageNumber - 1) * request.pageSize,
                PageSize = request.pageSize
            });

            if (attendanceRecords.Any())
            {
                return new ServiceResponse<IEnumerable<TransportAttendanceResponse>>(true, "Records Found", attendanceRecords, StatusCodes.Status200OK, totalCount);
            }
            else
            {
                return new ServiceResponse<IEnumerable<TransportAttendanceResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent, totalCount);
            }
        }

        public async Task<ServiceResponse<TransportAttendance>> GetTransportAttendanceById(int transportAttendanceId)
        {
            string sql = @"SELECT * FROM tbl_Transport_Attendance WHERE TransportAttendanceId = @TransportAttendanceId";
            var transportAttendance = await _dbConnection.QueryFirstOrDefaultAsync<TransportAttendance>(sql, new { TransportAttendanceId = transportAttendanceId });

            if (transportAttendance != null)
            {
                return new ServiceResponse<TransportAttendance>(true, "Record Found", transportAttendance, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<TransportAttendance>(false, "Record Not Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateTransportAttendanceStatus(int transportAttendanceId)
        {
            string sql = @"UPDATE tbl_Transport_Attendance SET IsActive = ~IsActive WHERE TransportAttendanceId = @TransportAttendanceId";
            var result = await _dbConnection.ExecuteAsync(sql, new { TransportAttendanceId = transportAttendanceId });

            if (result > 0)
            {
                return new ServiceResponse<bool>(true, "Status Updated Successfully", true, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<bool>(false, "Status Update Failed", false, StatusCodes.Status400BadRequest);
            }
        }
    }
}
