using Dapper;
using Microsoft.Extensions.Configuration;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.Repository.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace StudentManagement_API.Repository.Implementations
{
    public class StudentDashboardRepository : IStudentDashboardRepository
    {
        private readonly IDbConnection _dbConnection;

        // Use IConfiguration to create the connection internally.
        public StudentDashboardRepository(IConfiguration configuration)
        {
            _dbConnection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<GetStudentStatisticsResponse> GetStudentStatisticsAsync(GetStudentStatisticsRequest request)
        {
            string sql = @"
                SELECT 
                    COUNT(*) AS TotalStudents,
                    SUM(CASE WHEN gender_id = 1 THEN 1 ELSE 0 END) AS BoysCount,
                    SUM(CASE WHEN gender_id = 2 THEN 1 ELSE 0 END) AS GirlsCount
                FROM tbl_StudentMaster
                WHERE Institute_id = @InstituteID";

            var result = await _dbConnection.QueryFirstOrDefaultAsync(sql, new { InstituteID = request.InstituteID });

            int totalStudents = result.TotalStudents;
            int boysCount = result.BoysCount;
            int girlsCount = result.GirlsCount;

            double boysPercentage = totalStudents > 0 ? (boysCount * 100.0 / totalStudents) : 0;
            double girlsPercentage = totalStudents > 0 ? (girlsCount * 100.0 / totalStudents) : 0;

            return new GetStudentStatisticsResponse
            {
                TotalStudents = totalStudents,
                Boys = new GenderStatistics
                {
                    Count = boysCount,
                    Percentage = boysPercentage
                },
                Girls = new GenderStatistics
                {
                    Count = girlsCount,
                    Percentage = girlsPercentage
                }
            };
        }


        public async Task<GetStudentStatusStatisticsResponse> GetStudentStatusStatisticsAsync(GetStudentStatusStatisticsRequest request)
        {
            string sql = @"
                SELECT 
                    COUNT(*) AS Total,
                    SUM(CASE WHEN isActive = 1 THEN 1 ELSE 0 END) AS ActiveCount,
                    SUM(CASE WHEN isActive = 0 THEN 1 ELSE 0 END) AS InactiveCount
                FROM tbl_StudentMaster
                WHERE Institute_id = @InstituteID";

            var result = await _dbConnection.QueryFirstOrDefaultAsync(sql, new { InstituteID = request.InstituteID });

            int total = result.Total;
            int activeCount = result.ActiveCount;
            int inactiveCount = result.InactiveCount;

            double activePercentage = total > 0 ? (activeCount * 100.0 / total) : 0;
            double inactivePercentage = total > 0 ? (inactiveCount * 100.0 / total) : 0;

            return new GetStudentStatusStatisticsResponse
            {
                Active = new StatusStatistics
                {
                    Count = activeCount,
                    Percentage = activePercentage
                },
                Inactive = new StatusStatistics
                {
                    Count = inactiveCount,
                    Percentage = inactivePercentage
                }
            };
        }

        public async Task<GetStudentTypeStatisticsResponse> GetStudentTypeStatisticsAsync(GetStudentTypeStatisticsRequest request)
        {
            // Get total number of students for the given institute.
            string totalQuery = "SELECT COUNT(*) FROM tbl_StudentMaster WHERE Institute_id = @InstituteID";
            int totalStudents = await _dbConnection.ExecuteScalarAsync<int>(totalQuery, new { InstituteID = request.InstituteID });

            // Query by joining tbl_StudentMaster and tbl_StudentType.
            string sql = @"
                SELECT 
                    st.Student_Type_Name AS Type,
                    COUNT(*) AS Count,
                    CASE WHEN @TotalStudents = 0 THEN 0 
                         ELSE (COUNT(*) * 100.0 / @TotalStudents) 
                    END AS Percentage
                FROM tbl_StudentMaster sm
                JOIN tbl_StudentType st ON sm.StudentType_id = st.Student_Type_id
                WHERE sm.Institute_id = @InstituteID
                GROUP BY st.Student_Type_Name";

            var studentTypeStatistics = await _dbConnection.QueryAsync<StudentTypeStatistic>(sql, new
            {
                InstituteID = request.InstituteID,
                TotalStudents = totalStudents
            });

            return new GetStudentTypeStatisticsResponse
            {
                StudentType = studentTypeStatistics.AsList()
            };
        }

        public async Task<List<HouseWiseStudent>> GetHouseWiseStudentAsync(GetHouseWiseStudentRequest request)
        {
            // This query joins the student master and institute house tables to group by house name.
            string sql = @"
                SELECT 
                    ih.HouseName AS HouseType,
                    COUNT(*) AS StudentCount
                FROM tbl_StudentMaster sm
                JOIN tbl_InstituteHouse ih ON sm.Institute_house_id = ih.Institute_house_id
                WHERE sm.Institute_id = @InstituteID
                GROUP BY ih.HouseName";

            var result = await _dbConnection.QueryAsync<HouseWiseStudent>(sql, new { InstituteID = request.InstituteID });
            return result.AsList();
        }

        public async Task<List<StudentBirthday>> GetStudentBirthdaysAsync(GetStudentBirthdaysRequest request)
        {
            // This query filters students whose birthdays (month and day) match today's date.
            string sql = @"
            SELECT 
                RTRIM(LTRIM(sm.First_Name + ' ' + ISNULL(sm.Middle_Name + ' ', '') + sm.Last_Name)) AS StudentName,
                CONVERT(varchar(11), sm.Date_of_Birth, 106) AS BirthDay,
                c.class_name AS Class,
                s.section_name AS Section
            FROM tbl_StudentMaster sm
            JOIN tbl_Class c ON sm.class_id = c.class_id
            JOIN tbl_Section s ON sm.section_id = s.section_id
            WHERE sm.Institute_id = @InstituteID
              AND sm.Date_of_Birth IS NOT NULL
              AND sm.isActive = 1
              AND MONTH(sm.Date_of_Birth) = MONTH(GETDATE())
              AND DAY(sm.Date_of_Birth) = DAY(GETDATE())";

            var result = await _dbConnection.QueryAsync<StudentBirthday>(sql, new { InstituteID = request.InstituteID });
            return result.AsList();
        }

        public async Task<List<ClassWiseStudent>> GetClassWiseStudentsAsync(GetClassWiseStudentsRequest request)
        {
            // Query starting from tbl_Class with a LEFT JOIN on tbl_StudentMaster.
            string sql = @"
        SELECT 
            c.class_name AS ClassName,
            ISNULL(SUM(CASE WHEN sm.gender_id = 1 THEN 1 ELSE 0 END), 0) AS BoysCount,
            ISNULL(SUM(CASE WHEN sm.gender_id = 2 THEN 1 ELSE 0 END), 0) AS GirlsCount
        FROM tbl_Class c
        LEFT JOIN tbl_StudentMaster sm
            ON c.class_id = sm.class_id AND sm.Institute_id = @InstituteID
        WHERE c.institute_id = @InstituteID
        GROUP BY c.class_name
        ORDER BY c.class_name";

            var result = await _dbConnection.QueryAsync<ClassWiseStudent>(sql, new { InstituteID = request.InstituteID });
            return result.AsList();
        }

    }
}

