using Attendance_SE_API.Models;
using Attendance_SE_API.Repository.Interfaces;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.DTOs.Response;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Globalization;

namespace Attendance_SE_API.Repository.Implementations
{
    public class StudentAttendanceRepository : IStudentAttendanceRepository
    {
        private readonly IDbConnection _connection;

        public StudentAttendanceRepository(IDbConnection connection)
        {
            _connection = connection;
        } 

        public async Task<List<StudentAttendanceResponse>> GetAttendance(GetAttendanceRequest request)
        {
            // Parse the attendance date
            DateTime parsedDate;
            if (!DateTime.TryParseExact(request.AttendanceDate, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
            }

            // Validation based on attendanceTypeID
            if (request.AttendanceTypeID == 1 && request.TimeSlotTypeID == null)
            {
                throw new ArgumentException("TimeSlotTypeID should not be null when AttendanceTypeID is 1.");
            }

            if (request.AttendanceTypeID == 2 && request.SubjectID == null)
            {
                throw new ArgumentException("SubjectID should not be null when AttendanceTypeID is 2.");
            }

            // Start the query to include all students even if no attendance record exists
            string query = @"
                    DECLARE @AttendanceDate DATE = @AttendanceDateParam;  
                    DECLARE @ClassID INT = @ClassIDParam;                     
                    DECLARE @SectionID INT = @SectionIDParam;                    
                    DECLARE @InstituteID INT = @InstituteIDParam;                  
                    DECLARE @SubjectID INT = @SubjectIDParam;                    
                    DECLARE @TimeSlotTypeID INT = @TimeSlotTypeIDParam;             
                    DECLARE @AttendanceTypeID INT = @AttendanceTypeIDParam;          
                    DECLARE @AcademicYearCode VARCHAR(50) = @AcademicYearCodeParam;  -- Add AcademicYearCode parameter

                    ------------Part I------------
                    WITH PartI AS (
                        SELECT 
                            s.student_id, 
                            s.Admission_Number, 
                            s.Roll_Number, 
                            CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
                            ISNULL(a.StatusID, 0) AS StatusID, 
                            ISNULL(a.Remarks, '') AS Remarks 
                        FROM tbl_StudentMaster s
                        LEFT JOIN tblStudentAttendance a ON s.student_id = a.StudentID
                            AND a.AttendanceDate = @AttendanceDate
                            AND a.AcademicYearCode = @AcademicYearCode  -- Include AcademicYearCode in the condition
                        WHERE s.class_id = @ClassID 
                            AND s.section_id = @SectionID 
                            AND s.Institute_id = @InstituteID
                            AND (
                                (@AttendanceTypeID = 1 AND (@TimeSlotTypeID IS NULL OR a.TimeSlotTypeID = @TimeSlotTypeID))
                                OR (@AttendanceTypeID = 2 AND (@SubjectID IS NULL OR a.SubjectID = @SubjectID))
                                OR @AttendanceTypeID IS NULL  -- Allow all records if AttendanceTypeID is NULL
                            )
                    )

                    ------------Part II------------
                    SELECT 
                        s.student_id, 
                        s.Admission_Number, 
                        s.Roll_Number, 
                        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
                        '' AS StatusID, 
                        '' AS Remarks 
                    FROM tbl_StudentMaster s
                    WHERE s.class_id = @ClassID 
                        AND s.section_id = @SectionID 
                        AND s.Institute_id = @InstituteID
                        AND s.student_id NOT IN (SELECT student_id FROM PartI)  -- Exclude students in Part I

                    ------------Combine both parts using UNION------------
                    UNION

                    SELECT 
                        s.student_id, 
                        s.Admission_Number, 
                        s.Roll_Number, 
                        CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
                        ISNULL(a.StatusID, 0) AS StatusID, 
                        ISNULL(a.Remarks, '') AS Remarks 
                    FROM tbl_StudentMaster s
                    LEFT JOIN tblStudentAttendance a ON s.student_id = a.StudentID
                        AND a.AttendanceDate = @AttendanceDate
                        AND a.AcademicYearCode = @AcademicYearCode  -- Include AcademicYearCode in the condition
                    WHERE s.class_id = @ClassID 
                        AND s.section_id = @SectionID 
                        AND s.Institute_id = @InstituteID
                        AND (
                            (@AttendanceTypeID = 1 AND (@TimeSlotTypeID IS NULL OR a.TimeSlotTypeID = @TimeSlotTypeID))
                            OR (@AttendanceTypeID = 2 AND (@SubjectID IS NULL OR a.SubjectID = @SubjectID))
                            OR @AttendanceTypeID IS NULL  -- Allow all records if AttendanceTypeID is NULL
                        );";

            // Set parameters for the SQL query
            var parameters = new
            {
                AttendanceDateParam = parsedDate,
                ClassIDParam = request.ClassID,
                SectionIDParam = request.SectionID,
                InstituteIDParam = request.InstituteID,
                SubjectIDParam = request.SubjectID, // Ensure this is part of the request
                TimeSlotTypeIDParam = request.TimeSlotTypeID, // Ensure this is part of the request
                AttendanceTypeIDParam = request.AttendanceTypeID, // Ensure this is part of the request
                AcademicYearCodeParam = request.AcademicYearCode // Pass the AcademicYearCode parameter
            };

            // Execute the query
            var attendanceList = await _connection.QueryAsync<StudentAttendanceResponse>(query, parameters);
            return attendanceList.AsList();
        }

        public async Task<bool> SetAttendance(SetAttendanceRequest request)
        {
            // Parse the attendance date from the provided string format
            DateTime parsedDate;
            if (!DateTime.TryParseExact(request.AttendanceDate, "dd-MM-yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
            }

            foreach (var record in request.AttendanceRecords)
            {
                // Validate required fields based on AttendanceTypeID
                if (request.AttendanceTypeID == 1) // Assuming 1 = Date Wise
                {
                    if (!request.TimeSlotTypeID.HasValue)
                    {
                        throw new ArgumentException("TimeSlotTypeID is required for Date Wise attendance.");
                    }
                }
                else if (request.AttendanceTypeID == 2) // Assuming 2 = Subject Wise
                {
                    if (!request.SubjectID.HasValue)
                    {
                        throw new ArgumentException("SubjectID is required for Subject Wise attendance.");
                    }
                }

                // Create the attendance record object
                var attendance = new StudentAttendance
                {
                    InstituteID = request.InstituteID,
                    AttendanceTypeID = request.AttendanceTypeID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    AttendanceDate = parsedDate,
                    SubjectID = request.SubjectID ?? 0, // Use 0 if SubjectID is null
                    TimeSlotTypeID = request.TimeSlotTypeID ?? 0, // Use 0 if TimeSlotTypeID is null
                    IsMarkAsHoliday = request.IsMarkAsHoliday,
                    StudentID = record.StudentID,
                    StatusID = record.StatusID,
                    Remarks = record.Remarks,
                    AcademicYearCode = request.AcademicYearCode
                };

                // Delete existing records for the same key fields
                string deleteQuery = @"
            DELETE FROM tblStudentAttendance 
            WHERE StudentID = @StudentID 
              AND AttendanceDate = @AttendanceDate 
              AND ClassID = @ClassID 
              AND SectionID = @SectionID
              AND InstituteID = @InstituteID 
              AND AttendanceTypeID = @AttendanceTypeID 
              AND SubjectID = @SubjectID 
              AND TimeSlotTypeID = @TimeSlotTypeID
              AND AcademicYearCode = @AcademicYearCode";

                await _connection.ExecuteAsync(deleteQuery, attendance);

                // Insert the new record
                string insertQuery = @"
            INSERT INTO tblStudentAttendance 
                 (InstituteID, AttendanceTypeID, ClassID, SectionID, 
                  AttendanceDate, SubjectID, TimeSlotTypeID, IsMarkAsHoliday, 
                  StudentID, StatusID, Remarks, AcademicYearCode)
            VALUES 
                 (@InstituteID, @AttendanceTypeID, @ClassID, @SectionID, 
                  @AttendanceDate, @SubjectID, @TimeSlotTypeID, @IsMarkAsHoliday, 
                  @StudentID, @StatusID, @Remarks, @AcademicYearCode)";

                await _connection.ExecuteAsync(insertQuery, attendance);
            }

            return true;
        }
         

        //public async Task<bool> SetAttendance(SetAttendanceRequest request)
        //{
        //    // Parse the attendance date from the provided string format
        //    DateTime parsedDate;
        //    if (!DateTime.TryParseExact(request.AttendanceDate, "dd-MM-yyyy",
        //        CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        //    {
        //        throw new ArgumentException("Invalid date format. Please use DD-MM-YYYY.");
        //    }

        //    foreach (var record in request.AttendanceRecords)
        //    {
        //        // Check required fields based on AttendanceTypeID
        //        if (request.AttendanceTypeID == 1) // Assuming 1 = Date Wise
        //        {
        //            if (!request.TimeSlotTypeID.HasValue)
        //            {
        //                throw new ArgumentException("TimeSlotTypeID is required for Date Wise attendance.");
        //            }
        //        }
        //        else if (request.AttendanceTypeID == 2) // Assuming 2 = Subject Wise
        //        {
        //            if (!request.SubjectID.HasValue)
        //            {
        //                throw new ArgumentException("SubjectID is required for Subject Wise attendance.");
        //            }
        //        }

        //        var attendance = new StudentAttendance
        //        {
        //            InstituteID = request.InstituteID,
        //            AttendanceTypeID = request.AttendanceTypeID,
        //            ClassID = request.ClassID,
        //            SectionID = request.SectionID,
        //            AttendanceDate = parsedDate,
        //            SubjectID = request.SubjectID ?? 0, // Use 0 or any default value if SubjectID is null
        //            TimeSlotTypeID = request.TimeSlotTypeID ?? 0, // Use 0 or any default value if TimeSlotTypeID is null
        //            IsMarkAsHoliday = request.IsMarkAsHoliday,
        //            StudentID = record.StudentID,
        //            StatusID = record.StatusID,
        //            Remarks = record.Remarks,
        //            AcademicYearCode = request.AcademicYearCode // Set the AcademicYearCode
        //        };

        //        string query = @"INSERT INTO tblStudentAttendance 
        //                 (InstituteID, AttendanceTypeID, ClassID, SectionID, 
        //                  AttendanceDate, SubjectID, TimeSlotTypeID, IsMarkAsHoliday, 
        //                  StudentID, StatusID, Remarks, AcademicYearCode)
        //                 VALUES 
        //                 (@InstituteID, @AttendanceTypeID, @ClassID, @SectionID, 
        //                  @AttendanceDate, @SubjectID, @TimeSlotTypeID, @IsMarkAsHoliday, 
        //                  @StudentID, @StatusID, @Remarks, @AcademicYearCode)";

        //        await _connection.ExecuteAsync(query, attendance);
        //    }

        //    return true;
        //}

    }
}
