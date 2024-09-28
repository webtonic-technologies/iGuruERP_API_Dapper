using Dapper;
using Employee_API.DTOs;
using Employee_API.DTOs.ServiceResponse;
using Employee_API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Employee_API.Repository.Implementations
{
    public class EmployeeDashboardRepository : IEmployeeDashboardRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeDashboardRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<ServiceResponse<EmployeeGenderStatsResponse>> GetEmployeeGenderStats(int instituteId)
        {
            try
            {
                string sql = @"
                SELECT 
                    COUNT(CASE WHEN Gender_id = 1 THEN 1 END) AS MaleCount,
                    COUNT(CASE WHEN Gender_id = 2 THEN 1 END) AS FemaleCount
                FROM tbl_EmployeeProfileMaster
                WHERE Institute_id = @InstituteId;
            ";

                var result = await _connection.QueryFirstOrDefaultAsync<EmployeeGenderStats>(sql, new { InstituteId = instituteId });

                if (result == null)
                {
                    return new ServiceResponse<EmployeeGenderStatsResponse>(false, "No data found", null, 204);
                }

                int totalEmployees = result.MaleCount + result.FemaleCount;
                if (totalEmployees == 0)
                {
                    return new ServiceResponse<EmployeeGenderStatsResponse>(false, "No employees found", null, 204);
                }

                var response = new EmployeeGenderStatsResponse
                {
                    Male = result.MaleCount,
                    MalePercentage = $"{(double)result.MaleCount / totalEmployees:P0}",
                    Female = result.FemaleCount,
                    FemalePercentage = $"{(double)result.FemaleCount / totalEmployees:P0}"
                };

                return new ServiceResponse<EmployeeGenderStatsResponse>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeGenderStatsResponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<AgeGroupStatsResponse>>> GetEmployeeAgeGroupStats(int instituteId)
        {
            try
            {
                string sql = @"
        SELECT 
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 18 AND 25 THEN 1 ELSE 0 END) AS Age_18_25,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 26 AND 30 THEN 1 ELSE 0 END) AS Age_26_30,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 31 AND 35 THEN 1 ELSE 0 END) AS Age_31_35,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 36 AND 40 THEN 1 ELSE 0 END) AS Age_36_40,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 41 AND 45 THEN 1 ELSE 0 END) AS Age_41_45,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 46 AND 50 THEN 1 ELSE 0 END) AS Age_46_50,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 51 AND 55 THEN 1 ELSE 0 END) AS Age_51_55,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 56 AND 60 THEN 1 ELSE 0 END) AS Age_56_60,
            SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) > 60 THEN 1 ELSE 0 END) AS Age_60_Plus
        FROM tbl_EmployeeProfileMaster
        WHERE Institute_id = @InstituteId;
        ";

                var result = await _connection.QueryFirstOrDefaultAsync<AgeGroupStats>(sql, new { InstituteId = instituteId });

                if (result == null)
                {
                    return new ServiceResponse<List<AgeGroupStatsResponse>>(false, "No data found", null, 204);
                }

                // Preparing the response in the required format
                var response = new List<AgeGroupStatsResponse>
        {
            new AgeGroupStatsResponse { AgeGroup = "18-25", AgeCount = result.Age_18_25 },
            new AgeGroupStatsResponse { AgeGroup = "26-30", AgeCount = result.Age_26_30 },
            new AgeGroupStatsResponse { AgeGroup = "31-35", AgeCount = result.Age_31_35 },
            new AgeGroupStatsResponse { AgeGroup = "36-40", AgeCount = result.Age_36_40 },
            new AgeGroupStatsResponse { AgeGroup = "41-45", AgeCount = result.Age_41_45 },
            new AgeGroupStatsResponse { AgeGroup = "46-50", AgeCount = result.Age_46_50 },
            new AgeGroupStatsResponse { AgeGroup = "51-55", AgeCount = result.Age_51_55 },
            new AgeGroupStatsResponse { AgeGroup = "56-60", AgeCount = result.Age_56_60 },
            new AgeGroupStatsResponse { AgeGroup = "60+", AgeCount = result.Age_60_Plus }
        };

                return new ServiceResponse<List<AgeGroupStatsResponse>>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<AgeGroupStatsResponse>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<ExperienceRangeResponse>>> GetEmployeeExperienceStats(int instituteId)
        {
            try
            {
                string sql = @"
        WITH CurrentExperience AS (
            SELECT 
                emp.Employee_id,
                DATEDIFF(MONTH, TRY_CAST(emp.Date_of_Joining AS DATE), GETDATE()) AS CurrentExperienceMonths
            FROM 
                tbl_EmployeeProfileMaster emp
            WHERE 
                emp.Institute_id = @InstituteId
        ),
        PreviousExperience AS (
            SELECT
                we.Employee_id,
                COALESCE(SUM(TRY_CAST(we.Year AS INT) * 12 + TRY_CAST(we.Month AS INT)), 0) AS PreviousExperienceMonths
            FROM 
                tbl_WorkExperienceMaster we
            GROUP BY 
                we.Employee_id
        ),
        TotalExperience AS (
            SELECT
                cur.Employee_id,
                ISNULL(cur.CurrentExperienceMonths, 0) + ISNULL(prev.PreviousExperienceMonths, 0) AS TotalExperienceMonths
            FROM 
                CurrentExperience cur
            LEFT JOIN 
                PreviousExperience prev ON cur.Employee_id = prev.Employee_id
        )
        SELECT 
            SUM(CASE WHEN TotalExperienceMonths BETWEEN 0 AND 11 THEN 1 ELSE 0 END) AS Experience_0_1,
            SUM(CASE WHEN TotalExperienceMonths BETWEEN 12 AND 23 THEN 1 ELSE 0 END) AS Experience_1_2,
            SUM(CASE WHEN TotalExperienceMonths BETWEEN 24 AND 35 THEN 1 ELSE 0 END) AS Experience_2_3,
            SUM(CASE WHEN TotalExperienceMonths BETWEEN 36 AND 47 THEN 1 ELSE 0 END) AS Experience_3_4,
            SUM(CASE WHEN TotalExperienceMonths BETWEEN 48 AND 59 THEN 1 ELSE 0 END) AS Experience_4_5,
            SUM(CASE WHEN TotalExperienceMonths BETWEEN 60 AND 71 THEN 1 ELSE 0 END) AS Experience_5_6,
            SUM(CASE WHEN TotalExperienceMonths >= 72 THEN 1 ELSE 0 END) AS Experience_6_7
        FROM 
            TotalExperience;
        ";

                var result = await _connection.QueryFirstOrDefaultAsync<ExperienceStats>(sql, new { InstituteId = instituteId });

                if (result == null)
                {
                    return new ServiceResponse<List<ExperienceRangeResponse>>(false, "No data found", null, 204);
                }

                var response = new List<ExperienceRangeResponse>
        {
            new ExperienceRangeResponse { Experience = "0-1", Count = result.Experience_0_1 },
            new ExperienceRangeResponse { Experience = "1-2", Count = result.Experience_1_2 },
            new ExperienceRangeResponse { Experience = "2-3", Count = result.Experience_2_3 },
            new ExperienceRangeResponse { Experience = "3-4", Count = result.Experience_3_4 },
            new ExperienceRangeResponse { Experience = "4-5", Count = result.Experience_4_5 },
            new ExperienceRangeResponse { Experience = "5-6", Count = result.Experience_5_6 },
            new ExperienceRangeResponse { Experience = "6+", Count = result.Experience_6_7 }
        };

                return new ServiceResponse<List<ExperienceRangeResponse>>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ExperienceRangeResponse>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<DepartmentEmployeeResponse>> GetEmployeeDepartmentCounts(int instituteId)
        {
            try
            {
                string sql = @"
                SELECT 
                    d.DepartmentName,
                    COUNT(e.Employee_id) AS EmployeeCount
                FROM 
                    tbl_EmployeeProfileMaster e
                INNER JOIN 
                    tbl_Department d ON e.Department_id = d.Department_id
                WHERE 
                    e.Institute_id = @InstituteId
                GROUP BY 
                    d.DepartmentName;
            ";

                var departments = (await _connection.QueryAsync<DepartmentEmployeeCount>(sql, new { InstituteId = instituteId })).ToList();

                int totalEmployees = departments.Sum(d => d.EmployeeCount);

                var response = new DepartmentEmployeeResponse
                {
                    Departments = departments,
                    TotalEmployees = totalEmployees
                };

                return new ServiceResponse<DepartmentEmployeeResponse>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<DepartmentEmployeeResponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<EmployeeEventsResponse>> GetEmployeeEvents(DateTime date, int instituteId)
        {
            try
            {
                string birthdaySql = @"
                SELECT 
                    e.Employee_id AS EmployeeId,
                    e.First_Name AS FirstName,
                    e.Last_Name AS LastName,
                    e.Date_of_Birth AS EventDate
                FROM 
                    tbl_EmployeeProfileMaster e
                WHERE 
                    MONTH(e.Date_of_Birth) = MONTH(@Date) AND 
                    DAY(e.Date_of_Birth) = DAY(@Date) AND
                    e.Institute_id = @InstituteId;
            ";

                string workAnniversarySql = @"
                SELECT 
                    e.Employee_id AS EmployeeId,
                    e.First_Name AS FirstName,
                    e.Last_Name AS LastName,
                    e.Date_of_Joining AS EventDate
                FROM 
                    tbl_EmployeeProfileMaster e
                WHERE 
                    MONTH(e.Date_of_Joining) = MONTH(@Date) AND 
                    DAY(e.Date_of_Joining) = DAY(@Date) AND
                    e.Institute_id = @InstituteId;
            ";

                var birthdays = (await _connection.QueryAsync<EmployeeEvent>(birthdaySql, new { Date = date, InstituteId = instituteId })).ToList();
                var workAnniversaries = (await _connection.QueryAsync<EmployeeEvent>(workAnniversarySql, new { Date = date, InstituteId = instituteId })).ToList();

                var response = new EmployeeEventsResponse
                {
                    Birthdays = birthdays,
                    WorkAnniversaries = workAnniversaries
                };

                return new ServiceResponse<EmployeeEventsResponse>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EmployeeEventsResponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<List<GenderCountByDesignation>>> GetGenderCountByDesignation(int instituteId)
        {
            try
            {
                string sql = @"
                SELECT 
                    d.Designation_id AS DesignationId,
                    d.DesignationName AS Designation,
                    SUM(CASE WHEN e.Gender_id = 1 THEN 1 ELSE 0 END) AS MaleCount,
                    SUM(CASE WHEN e.Gender_id = 2 THEN 1 ELSE 0 END) AS FemaleCount
                FROM 
                    tbl_EmployeeProfileMaster e
                JOIN 
                    tbl_Designation d ON e.Designation_id = d.Designation_id
                WHERE 
                    e.Institute_id = @InstituteId
                GROUP BY 
                    d.Designation_id,
                    d.DesignationName;
            ";

                var result = (await _connection.QueryAsync<GenderCountByDesignation>(sql, new { InstituteId = instituteId })).ToList();

                return new ServiceResponse<List<GenderCountByDesignation>>(true, "Data found", result, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GenderCountByDesignation>>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<ActiveInactiveEmployeesResponse>> GetActiveInactiveEmployees(int instituteId)
        {
            try
            {
                string sql = @"
        SELECT 
            SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS ActiveCount,
            SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS InactiveCount,
            COUNT(*) AS TotalCount
        FROM 
            tbl_EmployeeProfileMaster
        WHERE 
            Institute_id = @InstituteId;
        ";

                var result = await _connection.QueryFirstOrDefaultAsync<EmployeeStatusData>(sql, new { InstituteId = instituteId });

                if (result == null || result.TotalCount == 0)
                {
                    return new ServiceResponse<ActiveInactiveEmployeesResponse>(true, "No data found", null, 200);
                }

                var response = new ActiveInactiveEmployeesResponse
                {
                    ActiveCount = result.ActiveCount,
                    InactiveCount = result.InactiveCount,
                    ActivePercentage = Math.Round((double)result.ActiveCount / result.TotalCount * 100, 0),
                    InactivePercentage = Math.Round((double)result.InactiveCount / result.TotalCount * 100, 0)
                };

                return new ServiceResponse<ActiveInactiveEmployeesResponse>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ActiveInactiveEmployeesResponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<AppUserNonAppUserResponse>> GetAppUsersNonAppUsersEmployees(int instituteId)
        {
            try
            {
                // SQL query to fetch the count of app users and non-app users
                string sql = @"
        SELECT 
            SUM(CASE WHEN IsAppUser = 1 THEN 1 ELSE 0 END) AS AppUsers,
            SUM(CASE WHEN IsAppUser = 0 THEN 1 ELSE 0 END) AS NonAppUsers
        FROM 
            tblUserLogs
        WHERE 
            UserTypeId = 1
            AND Institute_id = @InstituteId;";

                // Execute the query and get the result
                var result = await _connection.QueryFirstOrDefaultAsync<AppUserNonAppUserResponse>(sql, new { InstituteId = instituteId });

                // If no data is found, initialize with zero values
                if (result == null)
                {
                    result = new AppUserNonAppUserResponse { AppUsers = 0, NonAppUsers = 0 };
                }

                // Return the response with success
                return new ServiceResponse<AppUserNonAppUserResponse>(true, "Data fetched successfully", result, 200);
            }
            catch (Exception ex)
            {
                // Return an error response in case of exceptions
                return new ServiceResponse<AppUserNonAppUserResponse>(false, $"Error fetching data: {ex.Message}", null, 500);
            }
        }
    }
}