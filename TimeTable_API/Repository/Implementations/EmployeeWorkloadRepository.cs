﻿using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using TimeTable_API.DTOs.Requests;
using TimeTable_API.DTOs.Responses;
using TimeTable_API.DTOs.ServiceResponse;
using TimeTable_API.Repository.Interfaces;

namespace TimeTable_API.Repository.Implementations
{
    public class EmployeeWorkloadRepository : IEmployeeWorkloadRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeWorkloadRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        //public async Task<ServiceResponse<int>> AddUpdateWorkload(AddUpdateWorkloadRequest request)
        //{
        //    try
        //    {
        //        if (_connection.State != ConnectionState.Open)
        //        {
        //            _connection.Open();
        //        }

        //        using (var transaction = _connection.BeginTransaction())
        //        {
        //            if (request.WorkLoadID == null || request.WorkLoadID == 0)
        //            {
        //                // Insert new workload entry
        //                string insertSql = @"
        //                    INSERT INTO tblTimeTableWorkload (ClassID, SectionID, SubjectID, EmployeeID, WorkLoad)
        //                    VALUES (@ClassID, @SectionID, @SubjectID, @EmployeeID, @WorkLoad);
        //                    SELECT CAST(SCOPE_IDENTITY() as int);";

        //                int workloadID = await _connection.ExecuteScalarAsync<int>(insertSql, new
        //                {
        //                    request.ClassID,
        //                    request.SectionID,
        //                    request.SubjectID,
        //                    request.EmployeeID,
        //                    request.WorkLoad
        //                }, transaction);

        //                transaction.Commit();
        //                return new ServiceResponse<int>(true, "Workload added successfully", workloadID, 200);
        //            }
        //            else
        //            {
        //                // Update existing workload entry
        //                string updateSql = @"
        //                    UPDATE tblTimeTableWorkload
        //                    SET ClassID = @ClassID, SectionID = @SectionID, SubjectID = @SubjectID, EmployeeID = @EmployeeID, WorkLoad = @WorkLoad
        //                    WHERE WorkLoadID = @WorkLoadID;";

        //                await _connection.ExecuteAsync(updateSql, new
        //                {
        //                    request.WorkLoadID,
        //                    request.ClassID,
        //                    request.SectionID,
        //                    request.SubjectID,
        //                    request.EmployeeID,
        //                    request.WorkLoad
        //                }, transaction);

        //                transaction.Commit();
        //                return new ServiceResponse<int>(true, "Workload updated successfully", request.WorkLoadID.Value, 200);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResponse<int>(false, ex.Message, 0, 500);
        //    }
        //}

        public async Task<ServiceResponse<int>> AddUpdateWorkload(AddUpdateWorkloadRequest request)
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                using (var transaction = _connection.BeginTransaction())
                {
                    // Check if a record already exists for the same ClassID, SectionID, SubjectID, and EmployeeID
                    string checkExistingSql = @"
                SELECT WorkLoadID, WorkLoad 
                FROM tblTimeTableWorkload 
                WHERE ClassID = @ClassID 
                AND SectionID = @SectionID 
                AND SubjectID = @SubjectID 
                AND EmployeeID = @EmployeeID;";

                    var existingWorkload = await _connection.QueryFirstOrDefaultAsync<(int WorkLoadID, decimal WorkLoad)>(
                        checkExistingSql,
                        new
                        {
                            request.ClassID,
                            request.SectionID,
                            request.SubjectID,
                            request.EmployeeID
                        }, transaction);

                    if (existingWorkload.WorkLoadID > 0)
                    {
                        // If it exists and the request includes a WorkLoadID (update scenario), perform a direct update
                        if (request.WorkLoadID != null && request.WorkLoadID > 0)
                        {
                            // Update the workload with the new value (real update)
                            string updateWorkloadSql = @"
                        UPDATE tblTimeTableWorkload
                        SET WorkLoad = @NewWorkLoad
                        WHERE WorkLoadID = @WorkLoadID;";

                            await _connection.ExecuteAsync(updateWorkloadSql, new
                            {
                                NewWorkLoad = request.WorkLoad,
                                WorkLoadID = request.WorkLoadID
                            }, transaction);

                            transaction.Commit();
                            return new ServiceResponse<int>(true, "Workload updated successfully", request.WorkLoadID.Value, 200);
                        }
                        else
                        {
                            // Add the new workload value to the existing workload (adding scenario)
                            string updateWorkloadSql = @"
                        UPDATE tblTimeTableWorkload
                        SET WorkLoad = WorkLoad + @NewWorkLoad
                        WHERE WorkLoadID = @WorkLoadID;";

                            await _connection.ExecuteAsync(updateWorkloadSql, new
                            {
                                NewWorkLoad = request.WorkLoad,
                                WorkLoadID = existingWorkload.WorkLoadID
                            }, transaction);

                            transaction.Commit();
                            return new ServiceResponse<int>(true, "Workload added to the existing value successfully", existingWorkload.WorkLoadID, 200);
                        }
                    }
                    else
                    {
                        // If no existing record, insert new workload entry
                        string insertSql = @"
                    INSERT INTO tblTimeTableWorkload (ClassID, SectionID, SubjectID, EmployeeID, WorkLoad)
                    VALUES (@ClassID, @SectionID, @SubjectID, @EmployeeID, @WorkLoad);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                        int workloadID = await _connection.ExecuteScalarAsync<int>(insertSql, new
                        {
                            request.ClassID,
                            request.SectionID,
                            request.SubjectID,
                            request.EmployeeID,
                            request.WorkLoad
                        }, transaction);

                        transaction.Commit();
                        return new ServiceResponse<int>(true, "Workload added successfully", workloadID, 200);
                    }
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>(false, ex.Message, 0, 500);
            }
        }


        public async Task<ServiceResponse<EmployeeWorkloadResponse>> GetEmployeeWorkload(EmployeeWorkloadRequest request)
        {
            var response = new ServiceResponse<EmployeeWorkloadResponse>(false, "Failed to retrieve workload", new EmployeeWorkloadResponse(), 500);

            try
            {
                // Fetch Employee Details
                var employeeDetailsQuery = @"
            SELECT 
                emp.EmpPhoto,
                emp.First_Name + ' ' + emp.Last_Name AS EmployeeName,
                emp.Employee_code_id AS EmployeeCode,
                emp.EmailID,
                emp.mobile_number AS MobileNumber,
                dept.DepartmentName,
                desig.DesignationName
            FROM 
                tbl_EmployeeProfileMaster emp
            LEFT JOIN 
                tbl_Department dept ON emp.Department_id = dept.Department_id
            LEFT JOIN 
                tbl_Designation desig ON emp.Designation_id = desig.Designation_id
            WHERE 
                emp.Employee_id = @EmployeeID AND emp.Institute_id = @InstituteID;";

                var employeeDetails = await _connection.QueryFirstOrDefaultAsync<EmployeeDetailsResponse>(
                    employeeDetailsQuery,
                    new { request.EmployeeID, request.InstituteID }
                );

                // Fetch Workload and Session Details with DefineWorkLoad
                var workloadDetailsQuery = @"
            SELECT 
                cls.class_name AS Class,
                sec.section_name AS Section,
                sub.SubjectName AS Subject,
                COALESCE(wl.WorkLoad, 0) AS DefineWorkLoad, -- Get the workload from tblTimeTableWorkload
                COUNT(tsm.TTSessionID) AS AssignedSession
            FROM 
                tblTimeTableSessionSubjectEmployee tse
            JOIN 
                tblTimeTableSessionMapping tsm ON tse.TTSessionID = tsm.TTSessionID
            JOIN 
                tblTimeTableSessions ses ON tsm.SessionID = ses.SessionID
            JOIN 
                tblTimeTableClassSession tcs ON tsm.GroupID = tcs.GroupID 
            JOIN 
                tbl_Class cls ON tcs.ClassID = cls.class_id
            JOIN 
                tbl_Section sec ON tcs.SectionID = sec.section_id
            JOIN 
                tbl_Subjects sub ON tse.SubjectID = sub.SubjectID
            LEFT JOIN 
                tblTimeTableWorkload wl ON wl.ClassID = tcs.ClassID 
                AND wl.SectionID = tcs.SectionID 
                AND wl.SubjectID = tse.SubjectID 
                AND wl.EmployeeID = tse.EmployeeID -- Left join to get DefineWorkLoad from tblTimeTableWorkload
            WHERE 
                tse.EmployeeID = @EmployeeID 
            GROUP BY 
                cls.class_name, sec.section_name, sub.SubjectName, wl.WorkLoad;"; 
        
        var workloadDetails = await _connection.QueryAsync<WorkloadDetailsResponse>(
            workloadDetailsQuery,
            new { request.EmployeeID }
        );

                // Calculate Total Sessions based on tblTimeTableWorkload
                var totalSessionsQuery = @"
            SELECT 
                SUM(wl.WorkLoad) AS TotalSessions
            FROM 
                tblTimeTableWorkload wl
            WHERE 
                wl.EmployeeID = @EmployeeID;";

                var totalSessions = await _connection.ExecuteScalarAsync<decimal>(totalSessionsQuery, new { request.EmployeeID });

                // Calculate Assigned Sessions (already calculated based on the existing logic)
                var sessionCountQuery = @"
            SELECT 
                COUNT(tsm.TTSessionID) AS TotalSessions,
                SUM(CASE WHEN tse.EmployeeID = @EmployeeID THEN 1 ELSE 0 END) AS AssignedSessions
            FROM 
                tblTimeTableSessionMapping tsm
            JOIN 
                tblTimeTableSessions ses ON tsm.SessionID = ses.SessionID
            LEFT JOIN 
                tblTimeTableSessionSubjectEmployee tse ON tsm.TTSessionID = tse.TTSessionID
            JOIN 
                tblTimeTableClassSession tcs ON tsm.GroupID = tcs.GroupID
            WHERE 
                tsm.GroupID IN (SELECT GroupID FROM tblTimeTableGroups WHERE InstituteID = @InstituteID);";

                var sessionCount = await _connection.QueryFirstOrDefaultAsync<SessionCountResponse>(
                    sessionCountQuery,
                    new { request.EmployeeID, request.InstituteID }
                );

                // Update the totalSessions field based on the workload table
                sessionCount.TotalSessions = (int)totalSessions;

                // Combine results into the response object
                var result = new EmployeeWorkloadResponse
                {
                    EmployeeDetails = employeeDetails,
                    WorkloadDetails = workloadDetails.AsList(),
                    SessionCount = sessionCount
                };

                response = new ServiceResponse<EmployeeWorkloadResponse>(
                    true,
                    "Workload retrieved successfully",
                    result,
                    200
                );
            }
            catch (Exception ex)
            {
                response = new ServiceResponse<EmployeeWorkloadResponse>(
                    false,
                    ex.Message,
                    null,
                    500
                );
            }

            return response;
        }
    }
}
