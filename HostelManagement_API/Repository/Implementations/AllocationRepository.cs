using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using HostelManagement_API.Repository.Interfaces;
using System.Globalization;

namespace HostelManagement_API.Repository.Implementations
{
    public class AllocationRepository : IAllocationRepository
    {
        private readonly string _connectionString;

        public AllocationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ServiceResponse<IEnumerable<GetStudentResponse>>> GetStudentsByInstituteClassSection(GetStudentRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                // SQL Query to fetch the student details
                string sqlQuery = @"
            SELECT 
                sm.student_id AS StudentID, 
                CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName, 
                CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection, 
                sm.Admission_Number AS AdmissionNumber, 
                sm.Roll_Number AS RollNumber, 
                st.Student_Type_Name AS StudentType
            FROM tbl_StudentMaster sm
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_Section s ON sm.section_id = s.section_id
            INNER JOIN tbl_StudentType st ON sm.StudentType_id = st.Student_Type_ID
            WHERE sm.Institute_id = @InstituteID
            AND sm.class_id = @ClassID
            AND sm.section_id = @SectionID
            AND sm.IsActive = 1";

                // Get students from the database
                var students = await db.QueryAsync<GetStudentResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID
                });

                // Calculate the total number of records for pagination
                string countQuery = @"SELECT COUNT(*) FROM tbl_StudentMaster sm
                               WHERE sm.Institute_id = @InstituteID
                               AND sm.class_id = @ClassID
                               AND sm.section_id = @SectionID
                               AND sm.IsActive = 1";

                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new
                {
                    request.InstituteID,
                    request.ClassID,
                    request.SectionID
                });

                // Return the data wrapped in ServiceResponse
                return new ServiceResponse<IEnumerable<GetStudentResponse>>(true, "Students Retrieved Successfully", students, 200, totalCount);
            }
        }

        public async Task<ServiceResponse<string>> AllocateHostel(AllocateHostelRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Parse the AllocateDate and VacateDate strings to DateTime
                DateTime? parsedAllocateDate = null;
                DateTime? parsedVacateDate = null;

                if (!string.IsNullOrEmpty(request.AllocateDate))
                {
                    parsedAllocateDate = DateTime.ParseExact(request.AllocateDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(request.VacateDate))
                {
                    parsedVacateDate = DateTime.ParseExact(request.VacateDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                }

                string sqlQuery = @"
            INSERT INTO tblHostelRoomAllocation
            (StudentID, HostelID, RoomID, RoomBedID, AllocateDate, VacateDate, IsAllocated, IsVacated, InstituteID, EntryDate, AllotterID)
            VALUES
            (@StudentID, @HostelID, @RoomID, @RoomBedID, @AllocateDate, @VacateDate, @IsAllocated, @IsVacated, @InstituteID, @EntryDate, @AllotterID)";

                var result = await db.ExecuteAsync(sqlQuery, new
                {
                    request.StudentID,
                    request.HostelID,
                    request.RoomID,
                    request.RoomBedID,
                    AllocateDate = parsedAllocateDate,
                    VacateDate = parsedVacateDate,
                    request.IsAllocated,
                    request.IsVacated,
                    request.InstituteID,
                    EntryDate = System.DateTime.Now,  // Current date and time when the entry is created
                    request.AllotterID  // Include AllotterID in the query
                });

                if (result > 0)
                {
                    return new ServiceResponse<string>(true, "Hostel Allocated Successfully", "Success", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Allocation Failed", "Failure", 500);
                }
            }
        }

        public async Task<GetHostelHistoryResponse> GetHostelHistory(GetHostelHistoryRequest request)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"
            SELECT 
                sm.student_id AS StudentID, 
                CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName, 
                CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection, 
                sm.Roll_Number AS RollNumber, 
                sm.Admission_Number AS AdmissionNumber,
                hr.HostelID, h.HostelName, 
                hr.RoomID, r.RoomName, 
                hr.AllocateDate, hr.VacateDate,  
                CASE 
                    WHEN ep.Employee_id IS NOT NULL THEN CONCAT(ep.First_Name, ' ', ep.Last_Name) 
                    ELSE 'Not Available'
                END AS AllotterName
            FROM tblHostelRoomAllocation hr
            INNER JOIN tbl_StudentMaster sm ON hr.StudentID = sm.student_id
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_Section s ON sm.section_id = s.section_id
            INNER JOIN tblHostel h ON hr.HostelID = h.HostelID
            INNER JOIN tblRoom r ON hr.RoomID = r.RoomID
            LEFT JOIN tbl_EmployeeProfileMaster ep ON hr.AllotterID = ep.Employee_id
            WHERE sm.student_id = @StudentID AND sm.Institute_id = @InstituteID
            ORDER BY hr.AllocateDate ASC";

                var history = await db.QueryAsync<dynamic>(sqlQuery, new
                {
                    request.StudentID,
                    request.InstituteID
                });

                var hostelHistoryList = history.Select(h => new StudentHostelHistory
                {
                    HostelID = h.HostelID,
                    HostelName = h.HostelName,
                    RoomID = h.RoomID,
                    RoomName = h.RoomName,
                    AllocateDate = h.AllocateDate == null ? null : ((DateTime)h.AllocateDate).ToString("dd-MM-yyyy"),
                    VacateDate = h.VacateDate == null ? null : ((DateTime?)h.VacateDate)?.ToString("dd-MM-yyyy"),
                    AllotterName = h.AllotterName  // Ensure AllotterName is mapped here
                }).ToList();

                return new GetHostelHistoryResponse
                {
                    StudentID = request.StudentID,
                    StudentName = history.FirstOrDefault()?.StudentName ?? "Not Found",
                    ClassSection = history.FirstOrDefault()?.ClassSection ?? "Not Found",
                    RollNumber = history.FirstOrDefault()?.RollNumber ?? "Not Found",
                    AdmissionNumber = history.FirstOrDefault()?.AdmissionNumber ?? "Not Found", 
                    StudentHostels = hostelHistoryList
                };
            }
        }


        //public async Task<GetHostelHistoryResponse> GetHostelHistory(GetHostelHistoryRequest request)
        //{
        //    using (var db = new SqlConnection(_connectionString))
        //    {
        //        var sqlQuery = @"
        //    SELECT 
        //        sm.student_id AS StudentID, 
        //        CONCAT(sm.First_Name, ' ', sm.Last_Name) AS StudentName, 
        //        CONCAT(c.class_name, ' - ', s.section_name) AS ClassSection, 
        //        sm.Roll_Number, 
        //        hr.HostelID, h.HostelName, 
        //        hr.RoomID, r.RoomName, 
        //        hr.AllocateDate, hr.VacateDate
        //    FROM tblHostelRoomAllocation hr
        //    INNER JOIN tbl_StudentMaster sm ON hr.StudentID = sm.student_id
        //    INNER JOIN tbl_Class c ON sm.class_id = c.class_id
        //    INNER JOIN tbl_Section s ON sm.section_id = s.section_id
        //    INNER JOIN tblHostel h ON hr.HostelID = h.HostelID
        //    INNER JOIN tblRoom r ON hr.RoomID = r.RoomID
        //    WHERE sm.student_id = @StudentID AND sm.Institute_id = @InstituteID
        //    ORDER BY hr.tblHostelRoomAllocation ASC";

        //        var history = await db.QueryAsync<dynamic>(sqlQuery, new
        //        {
        //            request.StudentID,
        //            request.InstituteID
        //        });

        //        // Map dynamic result to the StudentHostelHistory model
        //        var hostelHistoryList = history.Select(h => new StudentHostelHistory
        //        {
        //            HostelID = h.HostelID,
        //            HostelName = h.HostelName,
        //            RoomID = h.RoomID,
        //            RoomName = h.RoomName,
        //            AllocateDate = h.AllocateDate == null ? (DateTime?)null : (DateTime)h.AllocateDate, // Safely handle null AllocateDate
        //            VacateDate = h.VacateDate == null ? (DateTime?)null : (DateTime?)h.VacateDate // Safely handle null VacateDate
        //        }).ToList();

        //        return new GetHostelHistoryResponse
        //        {
        //            StudentID = request.StudentID,
        //            StudentName = history.FirstOrDefault()?.StudentName ?? "Not Found",
        //            ClassSection = history.FirstOrDefault()?.ClassSection ?? "Not Found",
        //            RollNumber = history.FirstOrDefault()?.RollNumber ?? "Not Found",
        //            StudentHostels = hostelHistoryList
        //        };
        //    }
        //}

        public async Task<ServiceResponse<string>> VacateHostel(VacateHostelRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();

                // Convert the VacateDate string into DateTime for the SQL query
                DateTime vacateDate = DateTime.ParseExact(request.VacateDate, "dd-MM-yyyy", null);

                string sqlQuery = @"
                    UPDATE tblHostelRoomAllocation
                    SET VacateDate = @VacateDate, IsVacated = 1
                    WHERE StudentID = @StudentID AND InstituteID = @InstituteID
                    AND IsAllocated = 1 AND IsVacated = 0";

                var result = await db.ExecuteAsync(sqlQuery, new
                {
                    request.StudentID,
                    request.InstituteID,
                    VacateDate = vacateDate // Pass the DateTime object to the query
                });

                if (result > 0)
                {
                    return new ServiceResponse<string>(true, "Hostel Vacated Successfully", "Success", 200);
                }
                else
                {
                    return new ServiceResponse<string>(false, "Vacate Failed", "Failure", 500);
                }
            }
        }

        public async Task<IEnumerable<GetHostelResponse>> GetHostels(int instituteID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"
                    SELECT HostelID, HostelName
                    FROM tblHostel
                    WHERE InstituteID = @InstituteID
                      AND IsActive = 1";
                return await db.QueryAsync<GetHostelResponse>(sqlQuery, new { InstituteID = instituteID });
            }
        }

        public async Task<IEnumerable<GetHostelRoomsResponse>> GetHostelRooms(int instituteID, int hostelID)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"
                    SELECT RoomID, RoomName
                    FROM tblRoom
                    WHERE InstituteID = @InstituteID AND HostelID = @HostelID AND IsActive = 1";

                return await db.QueryAsync<GetHostelRoomsResponse>(sqlQuery, new { InstituteID = instituteID, HostelID = hostelID });
            }
        }

        public async Task<IEnumerable<GetHostelRoomBedsResponse>> GetHostelRoomBeds(int instituteID, int roomID)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var sqlQuery = @"
                    SELECT RoomBedID, RoomBedName, BedPosition
                    FROM tblRoomBeds
                    WHERE InstituteID = @InstituteID AND RoomID = @RoomID";

                return await db.QueryAsync<GetHostelRoomBedsResponse>(sqlQuery, new { InstituteID = instituteID, RoomID = roomID });
            }
        }
    }
}
