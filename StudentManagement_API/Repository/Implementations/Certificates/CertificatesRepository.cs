using System.Data.SqlClient;
using Dapper;
using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using StudentManagement_API.DTOs.Responses;

namespace StudentManagement_API.Repository.Implementations
{
    public class CertificatesRepository : ICertificatesRepository
    {
        private readonly string _connectionString;

        public CertificatesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                INSERT INTO tblCertificateTemplate
                (
                    TemplateName,
                    TemplateContent,
                    InstituteID,
                    UserID,
                    CreatedOn
                )
                VALUES
                (
                    @TemplateName,
                    @TemplateContent,
                    @InstituteID,
                    @UserID,
                    GETDATE()
                );
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            var parameters = new
            {
                request.TemplateName,
                request.TemplateContent,
                request.InstituteID,
                request.UserID
            };

            connection.Open();
            var id = await connection.QuerySingleAsync<int>(sql, parameters);
            return id;
        }

        public async Task<IEnumerable<GetCertificateTemplateResponse>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"
                SELECT 
                    ct.TemplateID, 
                    ct.TemplateName, 
                    CONCAT(ep.First_Name, ' ', ISNULL(ep.Middle_Name, ''), ' ', ep.Last_Name) AS UserName,
                    FORMAT(ct.CreatedOn, 'dd-MM-yyyy ''at'' hh:mm tt') AS CreatedOn
                FROM tblCertificateTemplate ct
                LEFT JOIN tbl_EmployeeProfileMaster ep ON ct.UserID = ep.Employee_id
                WHERE ct.InstituteID = @InstituteID
                  AND (@Search IS NULL OR @Search = '' OR ct.TemplateName LIKE '%' + @Search + '%')";

            connection.Open();
            var results = await connection.QueryAsync<GetCertificateTemplateResponse>(
                sql,
                new { InstituteID = request.InstituteID, Search = request.Search }
            );
            return results;
        }
    }
}
