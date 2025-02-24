using System.Data.SqlClient;
using Dapper;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using StudentManagement_API.DTOs.Responses;
using System.Globalization;

namespace StudentManagement_API.Repository.Implementations
{
    public class ApprovalsRepository : IApprovalsRepository
    {
        private readonly string _connectionString;

        public ApprovalsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreatePermissionSlipAsync(CreatePermissionSlipRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            INSERT INTO tbl_PermissionSlip
            (
                Student_Id,
                Student_Parent_Info_id, -- New column for ParentID
                Institute_id,
                RequestedBy,
                PickedUpBy,
                Reason,
                RequestedDateTime
            )
            VALUES
            (
                @StudentID,
                @ParentID,  -- New parameter
                @InstituteID,
                @RequestedBy,
                @PickedUpBy,
                @Reason,
                GETDATE()
            );
            SELECT CAST(SCOPE_IDENTITY() AS int);";

            var parameters = new
            {
                StudentID = request.StudentID,
                ParentID = request.ParentID, // New parameter value
                InstituteID = request.InstituteID,
                RequestedBy = request.RequestedBy,
                PickedUpBy = request.PickedUpBy,
                Reason = request.Reason
            };

            connection.Open();
            var id = await connection.QuerySingleAsync<int>(sql, parameters);
            return id;
        }

        public async Task<IEnumerable<GetPermissionSlipResponse>> GetPermissionSlipAsync(GetPermissionSlipRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            SELECT 
                ps.PermissionSlip_Id AS PermissionSlipID,
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                sm.Admission_Number AS AdmissionNo,
                c.class_name AS Class,
                sec.section_name AS Section,
                g.Gender_Type AS Gender,
                pt.parent_type AS RequestedBy,
                COALESCE(CONCAT(spi.First_Name, ' ', ISNULL(spi.Middle_Name, ''), ' ', spi.Last_Name), '') AS ParentName,
                FORMAT(ps.RequestedDateTime, 'dd-MM-yyyy ''at'' hh:mm tt') AS RequestedDateTime,
                ps.Reason,
                ps.PickedUpBy AS PickedUp,
                ps.Status AS Status
            FROM tbl_PermissionSlip ps
            INNER JOIN tbl_StudentMaster sm ON ps.Student_Id = sm.student_id
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_section sec ON sm.section_id = sec.section_id
            INNER JOIN tbl_Gender g ON sm.gender_id = g.Gender_id
            LEFT JOIN tbl_StudentParentsInfo spi ON ps.Student_Parent_Info_id = spi.Student_Parent_Info_id
            INNER JOIN tbl_ParentType pt ON ps.RequestedBy = pt.parent_type_id
            WHERE
              ps.Status Is Null
              AND ps.Institute_id = @InstituteID
              AND sm.class_id = @ClassID
              AND sm.section_id = @SectionID
              AND (
                    @Search IS NULL OR @Search = '' OR
                    CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) LIKE '%' + @Search + '%' OR
                    sm.Admission_Number LIKE '%' + @Search + '%'
              )";

            connection.Open();
            var result = await connection.QueryAsync<GetPermissionSlipResponse>(
                sql,
                new { InstituteID = request.InstituteID, ClassID = request.ClassID, SectionID = request.SectionID, Search = request.Search }
            );
            return result;
        }

        //public async Task<bool> ChangePermissionSlipStatusAsync(ChangePermissionSlipStatusRequest request)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    string sql = @"
        //        UPDATE tbl_PermissionSlip
        //        SET 
        //            Status = @Status,
        //            ModifiedDate = GETDATE()
        //        WHERE PermissionSlip_Id = @PermissionSlipID
        //          AND Institute_id = @InstituteID";

        //    connection.Open();
        //    var affectedRows = await connection.ExecuteAsync(sql, new
        //    {
        //        Status = request.Status,
        //        PermissionSlipID = request.PermissionSlipID,
        //        InstituteID = request.InstituteID
        //    });
        //    return affectedRows > 0;
        //}


        public async Task<bool> ChangePermissionSlipStatusAsync(ChangePermissionSlipStatusRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            UPDATE tbl_PermissionSlip
            SET 
                Status = @Status,
                ModifiedDate = GETDATE(),
                StatusUpdatedBy = @StatusUpdatedBy
            WHERE PermissionSlip_Id = @PermissionSlipID
              AND Institute_id = @InstituteID";

            connection.Open();
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                Status = request.Status,
                PermissionSlipID = request.PermissionSlipID,
                InstituteID = request.InstituteID,
                StatusUpdatedBy = request.StatusUpdatedBy
            });
            return affectedRows > 0;
        }
         
        public async Task<IEnumerable<GetApprovedHistoryResponse>> GetApprovedHistoryAsync(GetApprovedHistoryRequest request)
        {
            // Convert the string dates to DateTime objects using the exact format.
            var startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            SELECT 
                ps.PermissionSlip_Id AS PermissionSlipID,
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                sm.Admission_Number AS AdmissionNo,
                c.class_name AS Class,
                sec.section_name AS Section,
                FORMAT(ps.ModifiedDate, 'dd-MM-yyyy') AS ApprovalDate,
                pt.parent_type AS RequestedBy,
                COALESCE(CONCAT(spi.First_Name, ' ', ISNULL(spi.Middle_Name, ''), ' ', spi.Last_Name), '') AS ParentName,
                ps.Reason,
                ps.PickedUpBy AS PickedUpBy,
                CONCAT(ep.First_Name, ' ', ISNULL(ep.Last_Name, '')) AS ApprovedBy
            FROM tbl_PermissionSlip ps
            INNER JOIN tbl_StudentMaster sm ON ps.Student_Id = sm.student_id
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_section sec ON sm.section_id = sec.section_id
            INNER JOIN tbl_ParentType pt ON ps.RequestedBy = pt.parent_type_id
            LEFT JOIN tbl_StudentParentsInfo spi ON ps.Student_Parent_Info_id = spi.Student_Parent_Info_id
            LEFT JOIN tbl_EmployeeProfileMaster ep ON ps.StatusUpdatedBy = ep.Employee_id
            WHERE ps.Institute_id = @InstituteID
              AND sm.class_id = @ClassID
              AND sm.section_id = @SectionID
              AND ps.Status = 1
              AND ps.ModifiedDate >= @StartDate
              AND ps.ModifiedDate <= @EndDate
              AND (
                  @Search IS NULL OR @Search = '' OR
                  CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) LIKE '%' + @Search + '%' OR
                  sm.Admission_Number LIKE '%' + @Search + '%'
              )
            ORDER BY ps.ModifiedDate DESC";

            connection.Open();
            var result = await connection.QueryAsync<GetApprovedHistoryResponse>(
                sql,
                new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    StartDate = startDate,
                    EndDate = endDate,
                    Search = request.Search
                }
            );
            return result;
        }

        public async Task<IEnumerable<GetRejectedHistoryResponse>> GetRejectedHistoryAsync(GetRejectedHistoryRequest request)
        {
            // Convert string dates to DateTime using the "dd-MM-yyyy" format.
            var startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    ps.PermissionSlip_Id AS PermissionSlipID,
                    sm.student_id AS StudentID,
                    CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                    sm.Admission_Number AS AdmissionNo,
                    c.class_name AS Class,
                    sec.section_name AS Section,
                    FORMAT(ps.ModifiedDate, 'dd-MM-yyyy') AS RejectionDate,
                    pt.parent_type AS RequestedBy,
                    COALESCE(CONCAT(spi.First_Name, ' ', ISNULL(spi.Middle_Name, ''), ' ', spi.Last_Name), '') AS ParentName,
                    ps.Reason,
                    CONCAT(ep.First_Name, ' ', ISNULL(ep.Last_Name, '')) AS RejectedBy
                FROM tbl_PermissionSlip ps
                INNER JOIN tbl_StudentMaster sm ON ps.Student_Id = sm.student_id
                INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                INNER JOIN tbl_section sec ON sm.section_id = sec.section_id
                INNER JOIN tbl_ParentType pt ON ps.RequestedBy = pt.parent_type_id
                LEFT JOIN tbl_StudentParentsInfo spi ON ps.Student_Parent_Info_id = spi.Student_Parent_Info_id
                LEFT JOIN tbl_EmployeeProfileMaster ep ON ps.StatusUpdatedBy = ep.Employee_id
                WHERE ps.Institute_id = @InstituteID
                  AND sm.class_id = @ClassID
                  AND sm.section_id = @SectionID
                  AND ps.Status = 0
                  AND ps.ModifiedDate >= @StartDate
                  AND ps.ModifiedDate <= @EndDate
                  AND (
                      @Search IS NULL OR @Search = '' OR
                      CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) LIKE '%' + @Search + '%' OR
                      sm.Admission_Number LIKE '%' + @Search + '%'
                  )
                ORDER BY ps.ModifiedDate DESC";

            connection.Open();
            var result = await connection.QueryAsync<GetRejectedHistoryResponse>(
                sql,
                new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    StartDate = startDate,
                    EndDate = endDate,
                    Search = request.Search
                }
            );
            return result;
        }



        public async Task<IEnumerable<GetPermissionSlipExportResponse>> GetPermissionSlipExportAsync(GetPermissionSlipExportRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            SELECT 
                ps.PermissionSlip_Id AS PermissionSlipID,
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                sm.Admission_Number AS AdmissionNo,
                c.class_name AS Class,
                sec.section_name AS Section,
                g.Gender_Type AS Gender,
                pt.parent_type AS RequestedBy,
                COALESCE(CONCAT(spi.First_Name, ' ', ISNULL(spi.Middle_Name, ''), ' ', spi.Last_Name), '') AS ParentName,
                FORMAT(ps.RequestedDateTime, 'dd-MM-yyyy ''at'' hh:mm tt') AS RequestedDateTime,
                ps.Reason,
                ps.PickedUpBy AS PickedUp
            FROM tbl_PermissionSlip ps
            INNER JOIN tbl_StudentMaster sm ON ps.Student_Id = sm.student_id
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_section sec ON sm.section_id = sec.section_id
            INNER JOIN tbl_Gender g ON sm.gender_id = g.Gender_id
            LEFT JOIN tbl_StudentParentsInfo spi ON ps.Student_Parent_Info_id = spi.Student_Parent_Info_id
            INNER JOIN tbl_ParentType pt ON ps.RequestedBy = pt.parent_type_id
            WHERE
              ps.Status IS NULL
              AND ps.Institute_id = @InstituteID
              AND sm.class_id = @ClassID
              AND sm.section_id = @SectionID
              AND (
                    @Search IS NULL OR @Search = '' OR
                    CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) LIKE '%' + @Search + '%' OR
                    sm.Admission_Number LIKE '%' + @Search + '%'
              )";

            connection.Open();
            var result = await connection.QueryAsync<GetPermissionSlipExportResponse>(
                sql,
                new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    Search = request.Search
                }
            );
            return result;
        }

        public async Task<IEnumerable<GetApprovedHistoryExportResponse>> GetApprovedHistoryExportAsync(GetApprovedHistoryExportRequest request)
        {
            // Parse the date strings to DateTime objects using the "dd-MM-yyyy" format.
            var startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            using var connection = new SqlConnection(_connectionString);
            string sql = @"
            SELECT 
                ps.PermissionSlip_Id AS PermissionSlipID,
                sm.student_id AS StudentID,
                CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                sm.Admission_Number AS AdmissionNo,
                c.class_name AS Class,
                sec.section_name AS Section,
                FORMAT(ps.ModifiedDate, 'dd-MM-yyyy') AS ApprovalDate,
                pt.parent_type AS RequestedBy,
                COALESCE(CONCAT(spi.First_Name, ' ', ISNULL(spi.Middle_Name, ''), ' ', spi.Last_Name), '') AS ParentName,
                ps.Reason,
                ps.PickedUpBy AS PickedUp,
                CONCAT(ep.First_Name, ' ', ISNULL(ep.Last_Name, '')) AS ApprovedBy
            FROM tbl_PermissionSlip ps
            INNER JOIN tbl_StudentMaster sm ON ps.Student_Id = sm.student_id
            INNER JOIN tbl_Class c ON sm.class_id = c.class_id
            INNER JOIN tbl_section sec ON sm.section_id = sec.section_id
            INNER JOIN tbl_ParentType pt ON ps.RequestedBy = pt.parent_type_id
            LEFT JOIN tbl_StudentParentsInfo spi ON ps.Student_Parent_Info_id = spi.Student_Parent_Info_id
            LEFT JOIN tbl_EmployeeProfileMaster ep ON ps.StatusUpdatedBy = ep.Employee_id
            WHERE ps.Institute_id = @InstituteID
              AND sm.class_id = @ClassID
              AND sm.section_id = @SectionID
              AND ps.Status = 1
              AND ps.ModifiedDate >= @StartDate
              AND ps.ModifiedDate <= @EndDate
              AND (
                  @Search IS NULL OR @Search = '' OR
                  CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) LIKE '%' + @Search + '%' OR
                  sm.Admission_Number LIKE '%' + @Search + '%'
              )
            ORDER BY ps.ModifiedDate DESC";

            connection.Open();
            var result = await connection.QueryAsync<GetApprovedHistoryExportResponse>(
                sql,
                new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    StartDate = startDate,
                    EndDate = endDate,
                    Search = request.Search
                }
            );
            return result;
        }

        public async Task<IEnumerable<GetRejectedHistoryExportResponse>> GetRejectedHistoryExportAsync(GetRejectedHistoryExportRequest request)
        {
            // Convert StartDate and EndDate strings to DateTime objects.
            var startDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var endDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    ps.PermissionSlip_Id AS PermissionSlipID,
                    sm.student_id AS StudentID,
                    CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) AS StudentName,
                    sm.Admission_Number AS AdmissionNo,
                    c.class_name AS Class,
                    sec.section_name AS Section,
                    FORMAT(ps.ModifiedDate, 'dd-MM-yyyy') AS RejectionDate,
                    pt.parent_type AS RequestedBy,
                    COALESCE(CONCAT(spi.First_Name, ' ', ISNULL(spi.Middle_Name, ''), ' ', spi.Last_Name), '') AS ParentName,
                    ps.Reason,
                    CONCAT(ep.First_Name, ' ', ISNULL(ep.Last_Name, '')) AS RejectedBy
                FROM tbl_PermissionSlip ps
                INNER JOIN tbl_StudentMaster sm ON ps.Student_Id = sm.student_id
                INNER JOIN tbl_Class c ON sm.class_id = c.class_id
                INNER JOIN tbl_section sec ON sm.section_id = sec.section_id
                INNER JOIN tbl_ParentType pt ON ps.RequestedBy = pt.parent_type_id
                LEFT JOIN tbl_StudentParentsInfo spi ON ps.Student_Parent_Info_id = spi.Student_Parent_Info_id
                LEFT JOIN tbl_EmployeeProfileMaster ep ON ps.StatusUpdatedBy = ep.Employee_id
                WHERE ps.Institute_id = @InstituteID
                  AND sm.class_id = @ClassID
                  AND sm.section_id = @SectionID
                  AND ps.Status = 0
                  AND ps.ModifiedDate >= @StartDate
                  AND ps.ModifiedDate <= @EndDate
                  AND (
                      @Search IS NULL OR @Search = '' OR
                      CONCAT(sm.First_Name, ' ', ISNULL(sm.Middle_Name, ''), ' ', sm.Last_Name) LIKE '%' + @Search + '%' OR
                      sm.Admission_Number LIKE '%' + @Search + '%'
                  )
                ORDER BY ps.ModifiedDate DESC";

            connection.Open();
            var result = await connection.QueryAsync<GetRejectedHistoryExportResponse>(
                sql,
                new
                {
                    InstituteID = request.InstituteID,
                    ClassID = request.ClassID,
                    SectionID = request.SectionID,
                    StartDate = startDate,
                    EndDate = endDate,
                    Search = request.Search
                }
            );
            return result;
        }
    }
}
