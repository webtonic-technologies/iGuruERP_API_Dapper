using Dapper;
using Infirmary_API.DTOs.Response;
using Infirmary_API.DTOs.ServiceResponse;
using Infirmary_API.Repositories.Interfaces;
using System.Data;

namespace Infirmary_API.Repositories.Implementations
{
    public class StudentVaccinationFetchRepository : IStudentVaccinationFetchRepository
    {
        private readonly IDbConnection _connection;

        public StudentVaccinationFetchRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponseFetch<List<AcademicYearFetchResponse>>> GetAcademicYearFetch()
        {
            try
            {
                string sql = "SELECT AcademicYearID, AcademicYear FROM tblAcademicYearMaster";
                var result = await _connection.QueryAsync<AcademicYearFetchResponse>(sql);

                if (result != null && result.Any())
                {
                    return new ServiceResponseFetch<List<AcademicYearFetchResponse>>(
                        true, "Records found", result.ToList(), 200, result.Count());
                }
                else
                {
                    return new ServiceResponseFetch<List<AcademicYearFetchResponse>>(
                        false, "No records found", null, 204, 0);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponseFetch<List<AcademicYearFetchResponse>>(
                    false, ex.Message, null, 500, 0);
            }
        }

        public async Task<ServiceResponseFetch<List<ClassSectionFetchResponse>>> GetClassSectionFetch()
        {
            try
            {
                string sql = @"
                    SELECT 
                        c.class_id AS ClassID, 
                        c.class_name + '-' + s.section_name AS ClassSection 
                    FROM 
                        tbl_Class c 
                    INNER JOIN 
                        tbl_Section s ON c.class_id = s.class_id";

                var result = await _connection.QueryAsync<ClassSectionFetchResponse>(sql);

                if (result != null && result.Any())
                {
                    return new ServiceResponseFetch<List<ClassSectionFetchResponse>>(
                        true, "Records found", result.ToList(), 200, result.Count());
                }
                else
                {
                    return new ServiceResponseFetch<List<ClassSectionFetchResponse>>(
                        false, "No records found", null, 204, 0);
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponseFetch<List<ClassSectionFetchResponse>>(
                    false, ex.Message, null, 500, 0);
            }
        }
    }
}
