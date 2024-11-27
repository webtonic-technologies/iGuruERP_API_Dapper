using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Responses;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.ServiceResponse;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks; 


namespace Attendance_SE_API.Repository.Implementations
{
    public class MarkAttendanceRepository : IMarkAttendanceRepository
    {
        private readonly IDbConnection _connection;

        public MarkAttendanceRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<AttendanceTypeResponse>>> GetAttendanceType()
        {
            try
            {
                // Query to fetch all attendance types
                string query = "SELECT AttendanceTypeID, AttendanceType FROM tblAttendanceTypeMaster";

                // Execute the query and map the result
                var result = await _connection.QueryAsync<AttendanceTypeResponse>(query);

                return new ServiceResponse<List<AttendanceTypeResponse>>(true, "Attendance types found", result.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AttendanceTypeResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<ServiceResponse<List<TimeSlotTypeResponse>>> GetTimeSlotType()
        {
            try
            {
                // Query to fetch all time slot types
                string query = "SELECT TimeSlotTypeID, TimeSlotType, AttendanceScore FROM tblTimeSlotType";

                // Execute the query and map the result
                var result = await _connection.QueryAsync<TimeSlotTypeResponse>(query);

                return new ServiceResponse<List<TimeSlotTypeResponse>>(true, "Time Slot Types found", result.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<TimeSlotTypeResponse>>(false, ex.Message, null, 500);
            }
        }
         
        public async Task<ServiceResponse<List<AttendanceSubjectsResponse>>> GetAttendanceSubjects(AttendanceSubjectsRequest request)
        {
            try
            {
                // SQL query to fetch subjects based on InstituteID, ClassID, and SectionID
                string query = @"
                    SELECT s.SubjectID, s.SubjectName
                    FROM tbl_Subjects s
                    INNER JOIN tbl_ClassSectionSubjectMapping csm
                        ON s.SubjectID = csm.SubjectId
                    WHERE s.InstituteId = @InstituteID
                        AND csm.class_id = @ClassID
                        AND csm.section_id = @SectionID
                        AND s.IsDeleted = 0";

                var result = await _connection.QueryAsync<AttendanceSubjectsResponse>(query, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID
                });

                return new ServiceResponse<List<AttendanceSubjectsResponse>>(true, "Subjects found", result.AsList(), 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AttendanceSubjectsResponse>>(false, ex.Message, null, 500);
            }
        }
    }
}
