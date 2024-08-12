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
        public async Task<ServiceResponse<AgeGroupStatsResponse>> GetEmployeeAgeGroupStats(int instituteId)
        {
            try
            {
                string sql = @"
                SELECT 
                    SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 18 AND 30 THEN 1 ELSE 0 END) AS Age_18_30,
                    SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 30 AND 45 THEN 1 ELSE 0 END) AS Age_30_45,
                    SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) BETWEEN 45 AND 60 THEN 1 ELSE 0 END) AS Age_45_60,
                    SUM(CASE WHEN DATEDIFF(YEAR, Date_of_Birth, GETDATE()) > 60 THEN 1 ELSE 0 END) AS Age_60_Plus
                FROM tbl_EmployeeProfileMaster
                WHERE Institute_id = @InstituteId;
            ";

                var result = await _connection.QueryFirstOrDefaultAsync<AgeGroupStats>(sql, new { InstituteId = instituteId });

                if (result == null)
                {
                    return new ServiceResponse<AgeGroupStatsResponse>(false, "No data found", null, 204);
                }

                var response = new AgeGroupStatsResponse
                {
                    Age_18_30 = result.Age_18_30,
                    Age_30_45 = result.Age_30_45,
                    Age_45_60 = result.Age_45_60,
                    Age_60_Plus = result.Age_60_Plus
                };

                return new ServiceResponse<AgeGroupStatsResponse>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AgeGroupStatsResponse>(false, ex.Message, null, 500);
            }
        }
        public async Task<ServiceResponse<ExperienceStatsResponse>> GetEmployeeExperienceStats(int instituteId)
        {
            try
            {
                string sql = @"
                SELECT 
                    SUM(CASE WHEN DATEDIFF(MONTH, Date_of_Joining, GETDATE()) BETWEEN 0 AND 11 THEN 1 ELSE 0 END) AS Experience_0_1,
                    SUM(CASE WHEN DATEDIFF(MONTH, Date_of_Joining, GETDATE()) BETWEEN 12 AND 23 THEN 1 ELSE 0 END) AS Experience_1_2,
                    SUM(CASE WHEN DATEDIFF(MONTH, Date_of_Joining, GETDATE()) BETWEEN 24 AND 35 THEN 1 ELSE 0 END) AS Experience_2_3,
                    SUM(CASE WHEN DATEDIFF(MONTH, Date_of_Joining, GETDATE()) BETWEEN 36 AND 47 THEN 1 ELSE 0 END) AS Experience_3_4,
                    SUM(CASE WHEN DATEDIFF(MONTH, Date_of_Joining, GETDATE()) BETWEEN 48 AND 59 THEN 1 ELSE 0 END) AS Experience_4_5,
                    SUM(CASE WHEN DATEDIFF(MONTH, Date_of_Joining, GETDATE()) BETWEEN 60 AND 71 THEN 1 ELSE 0 END) AS Experience_5_6,
                    SUM(CASE WHEN DATEDIFF(MONTH, Date_of_Joining, GETDATE()) >= 72 THEN 1 ELSE 0 END) AS Experience_6_7,
                    MAX(DATEDIFF(MONTH, Date_of_Joining, GETDATE())) AS MaxExperience,
                    MIN(DATEDIFF(MONTH, Date_of_Joining, GETDATE())) AS MinExperience,
                    AVG(DATEDIFF(MONTH, Date_of_Joining, GETDATE())) AS AvgExperience
                FROM tbl_EmployeeProfileMaster
                WHERE Institute_id = @InstituteId;
            ";

                var result = await _connection.QueryFirstOrDefaultAsync<ExperienceStats>(sql, new { InstituteId = instituteId });

                if (result == null)
                {
                    return new ServiceResponse<ExperienceStatsResponse>(false, "No data found", null, 204);
                }

                var response = new ExperienceStatsResponse
                {
                    Experience_0_1 = result.Experience_0_1,
                    Experience_1_2 = result.Experience_1_2,
                    Experience_2_3 = result.Experience_2_3,
                    Experience_3_4 = result.Experience_3_4,
                    Experience_4_5 = result.Experience_4_5,
                    Experience_5_6 = result.Experience_5_6,
                    Experience_6_7 = result.Experience_6_7,
                    MaxExperience = $"{result.MaxExperience / 12} years {result.MaxExperience % 12} months",
                    MinExperience = $"{result.MinExperience / 12} years {result.MinExperience % 12} months",
                    AvgExperience = $"{Math.Floor(result.AvgExperience / 12)} years {Math.Floor(result.AvgExperience % 12)} months"
                };

                return new ServiceResponse<ExperienceStatsResponse>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ExperienceStatsResponse>(false, ex.Message, null, 500);
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
                    ActivePercentage = (double)result.ActiveCount / result.TotalCount * 100,
                    InactivePercentage = (double)result.InactiveCount / result.TotalCount * 100
                };

                return new ServiceResponse<ActiveInactiveEmployeesResponse>(true, "Data found", response, 200);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ActiveInactiveEmployeesResponse>(false, ex.Message, null, 500);
            }
        }
    }
}