using System.Data.SqlClient;
using System.Data;
using Dapper;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Attendance_SE_API.DTOs.Requests;
using Attendance_SE_API.Repository.Interfaces;

namespace Attendance_SE_API.Repository.Implementations
{
    public class StudentImportRepository : IStudentImportRepository
    {
        private readonly IDbConnection _dbConnection;

        public StudentImportRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        //public DataTable GetAttendanceData(StudentImportRequest request)
        //{
        //    // Ensure valid dates are parsed
        //    if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
        //    {
        //        throw new ArgumentException("StartDate and EndDate must not be null or empty.");
        //    }

        //    var parameters = new
        //    {
        //        ClassID = request.ClassID,
        //        SectionID = request.SectionID,
        //        StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
        //        EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
        //        InstituteID = request.InstituteID,
        //        TimeSlotTypeID = request.TimeSlotTypeID,
        //        AcademicYearCode = request.AcademicYearCode
        //    };

        //    string query = @"
        //    WITH AttendanceData AS (
        //        SELECT 
        //            s.student_id AS StudentID, 
        //            s.class_id AS ClassID,
        //            s.section_id AS SectionID,
        //            s.Admission_Number AS AdmissionNumber, 
        //            s.Roll_Number AS RollNumber, 
        //            CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
        //            sp.Mobile_Number AS MobileNumber,  
        //            a.AttendanceDate,
        //            sas.StatusName AS AttendanceStatus  
        //        FROM tbl_StudentMaster s
        //        LEFT JOIN tblStudentAttendance a ON s.student_id = a.StudentID 
        //            AND a.AttendanceDate BETWEEN @StartDate AND @EndDate
        //            AND a.TimeSlotTypeID = @TimeSlotTypeID
        //            AND a.AcademicYearCode = @AcademicYearCode
        //        LEFT JOIN tbl_StudentParentsInfo sp ON s.student_id = sp.Student_id AND sp.Parent_Type_id = 1
        //        LEFT JOIN tblStudentAttendanceStatus sas ON a.StatusID = sas.StatusID
        //        WHERE s.class_id = @ClassID 
        //            AND s.section_id = @SectionID 
        //            AND s.Institute_id = @InstituteID
        //    )
        //    SELECT 
        //        StudentID,ClassID, SectionID,
        //        AdmissionNumber,
        //        RollNumber,
        //        StudentName,
        //        MobileNumber,
        //        AttendanceDate,
        //        AttendanceStatus
        //    FROM AttendanceData
        //    ORDER BY StudentName, AttendanceDate";

        //    var result = _dbConnection.Query(query, parameters);

        //    // Debugging: Check if the result contains data
        //    if (result.Any())
        //    {
        //        Console.WriteLine("Data found, continuing...");
        //    }
        //    else
        //    {
        //        Console.WriteLine("No data found.");
        //    }

        //    // Create a new DataTable to hold the results
        //    DataTable dataTable = new DataTable();
        //    if (result.Any())
        //    {
        //        var firstRow = result.First();
        //        foreach (var column in firstRow)
        //        {
        //            dataTable.Columns.Add(column.Key);
        //        }

        //        // Add rows to the DataTable
        //        foreach (var row in result)
        //        {
        //            var dataRow = dataTable.NewRow();
        //            foreach (var column in row)
        //            {
        //                dataRow[column.Key] = column.Value;
        //            }
        //            dataTable.Rows.Add(dataRow);
        //        }
        //    }

        //    return dataTable;
        //}



        public DataTable GetAttendanceData(StudentImportRequest request)
        {
            // Ensure valid dates are parsed
            if (string.IsNullOrEmpty(request.StartDate) || string.IsNullOrEmpty(request.EndDate))
            {
                throw new ArgumentException("StartDate and EndDate must not be null or empty.");
            }

            var parameters = new
            {
                ClassID = request.ClassID,
                SectionID = request.SectionID,
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                InstituteID = request.InstituteID,
                TimeSlotTypeID = request.TimeSlotTypeID,
                AcademicYearCode = request.AcademicYearCode
            };

            // New query as per your request
            string query = @"
    WITH AttendanceData AS (
        SELECT 
            s.student_id AS StudentID, 
            s.class_id AS ClassID,
            s.section_id AS SectionID,
            s.Admission_Number AS AdmissionNumber, 
            s.Roll_Number AS RollNumber, 
            CONCAT(s.First_Name, ' ', s.Last_Name) AS StudentName, 
            sp.Mobile_Number AS MobileNumber
        FROM tbl_StudentMaster s 
        LEFT JOIN tbl_StudentParentsInfo sp ON s.student_id = sp.Student_id AND sp.Parent_Type_id = 1 
        WHERE s.class_id = @ClassID 
            AND s.section_id = @SectionID 
            AND s.Institute_id = @InstituteID
            AND s.AcademicYearCode = @AcademicYearCode
    )
    SELECT 
        StudentID, ClassID, SectionID,
        AdmissionNumber, RollNumber,
        StudentName, MobileNumber
    FROM AttendanceData
    ORDER BY StudentName";

            var result = _dbConnection.Query(query, parameters);

            // Debugging: Check if the result contains data
            if (result.Any())
            {
                Console.WriteLine("Data found, continuing...");
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            // Create a new DataTable to hold the results
            DataTable dataTable = new DataTable();
            if (result.Any())
            {
                var firstRow = result.First();
                foreach (var column in firstRow)
                {
                    dataTable.Columns.Add(column.Key);
                }

                // Add rows to the DataTable
                foreach (var row in result)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var column in row)
                    {
                        dataRow[column.Key] = column.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }


        // Method to get status data for Sheet 2
        public DataTable GetStatusData()
        {
            string query = @"
            SELECT StatusID, StatusName, ShortName 
            FROM tblStudentAttendanceStatus 
            WHERE IsActive = 1 AND IsDefault = 1
            UNION ALL 
            SELECT StatusID, StatusName, ShortName 
            FROM tblStudentAttendanceStatus 
            WHERE IsActive = 1 AND IsDefault = 0 AND InstituteID = 6";

            var result = _dbConnection.Query(query);

            // Create a new DataTable to hold the results
            DataTable dataTable = new DataTable();
            if (result.Any())
            {
                var firstRow = result.First();
                foreach (var column in firstRow)
                {
                    dataTable.Columns.Add(column.Key);
                }

                // Add rows to the DataTable
                foreach (var row in result)
                {
                    var dataRow = dataTable.NewRow();
                    foreach (var column in row)
                    {
                        dataRow[column.Key] = column.Value;
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        public async Task DeleteStudentAttendanceData(
            int instituteID,
            string attendanceDate,
            string classID,
            string sectionID,
            string studentID,
            string academicYearCode
)
        {
            // Convert attendanceDate from "Dec 17, Tue" to "DD-MM-YYYY"
            DateTime parsedDate;
            if (!DateTime.TryParseExact(attendanceDate, "MMM dd, ddd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                throw new ArgumentException("Invalid date format. Expected format is 'MMM dd, ddd' (e.g., 'Dec 17, Tue').");
            }

            string query = @"
            DELETE FROM tblStudentAttendance 
            WHERE StudentID = @StudentID 
                AND AttendanceDate = @AttendanceDate 
                AND ClassID = @ClassID 
                AND SectionID = @SectionID
                AND InstituteID = @InstituteID 
                AND AcademicYearCode = @AcademicYearCode";

            await _dbConnection.ExecuteAsync(query, new
            {
                InstituteID = instituteID,
                AttendanceDate = parsedDate.ToString("yyyy-MM-dd"), // Convert to 'yyyy-MM-dd' format for SQL Server
                ClassID = classID,
                SectionID = sectionID,
                StudentID = studentID,
                AcademicYearCode = academicYearCode
            });
        }

        public async Task InsertStudentAttendanceData(
            int instituteID,
            int attendanceTypeID,
            string classID,
            string sectionID,
            string attendanceDate,
            int subjectID,
            int timeSlotTypeID,
            bool isMarkAsHoliday,
            string studentID,
            string statusID,
            string remarks,
            string academicYearCode
        )
        {
            // Convert attendanceDate from "Dec 17, Tue" to "DD-MM-YYYY"
            DateTime parsedDate;
            if (!DateTime.TryParseExact(attendanceDate, "MMM dd, ddd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                throw new ArgumentException("Invalid date format. Expected format is 'MMM dd, ddd' (e.g., 'Dec 17, Tue').");
            }

            string query = @"
            INSERT INTO tblStudentAttendance 
            (InstituteID, AttendanceTypeID, ClassID, SectionID, 
             AttendanceDate, SubjectID, TimeSlotTypeID, IsMarkAsHoliday, 
             StudentID, StatusID, Remarks, AcademicYearCode, IsImported)
            VALUES 
            (@InstituteID, @AttendanceTypeID, @ClassID, @SectionID, 
             @AttendanceDate, @SubjectID, @TimeSlotTypeID, @IsMarkAsHoliday, 
             @StudentID, @StatusID, @Remarks, @AcademicYearCode, 1)";

            await _dbConnection.ExecuteAsync(query, new
            {
                InstituteID = instituteID,
                AttendanceTypeID = attendanceTypeID,
                ClassID = classID,
                SectionID = sectionID,
                AttendanceDate = parsedDate.ToString("yyyy-MM-dd"), // Convert to 'yyyy-MM-dd' format for SQL Server
                SubjectID = subjectID,
                TimeSlotTypeID = timeSlotTypeID,
                IsMarkAsHoliday = isMarkAsHoliday,
                StudentID = studentID,
                StatusID = statusID,
                Remarks = remarks,
                AcademicYearCode = academicYearCode
            });
        }

    }
}
