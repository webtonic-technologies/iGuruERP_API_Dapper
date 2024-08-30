using Dapper;
using InfirmaryVisit_API.DTOs.Response;
using InfirmaryVisit_API.DTOs.ServiceResponse;
using InfirmaryVisit_API.Repositories.Interfaces;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace InfirmaryVisit_API.Repositories.Implementations
{
    public class InfirmaryVisitFetchRepository : IInfirmaryVisitFetchRepository
    {
        private readonly IDbConnection _connection;

        public InfirmaryVisitFetchRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponseFetch<List<StudentInfoFetchResponse>>> GetAllStudentInfoFetch(int instituteId)
        {
            string query = @"
            SELECT 
                s.First_Name + ' ' + s.Last_Name AS StudentName,
                c.class_name + '-' + sec.section_name AS ClassSection,
                s.Admission_Number AS AdmissionNumber,
                s.Roll_Number AS RollNumber
            FROM tbl_StudentMaster s
            INNER JOIN tbl_Class c ON s.class_id = c.class_id
            INNER JOIN tbl_Section sec ON s.section_id = sec.section_id
            WHERE s.Institute_id = @InstituteID";

            var result = await _connection.QueryAsync<StudentInfoFetchResponse>(query, new { InstituteID = instituteId });

            return new ServiceResponseFetch<List<StudentInfoFetchResponse>>(
                true,
                "Records found",
                result.ToList(),
                200
            );
        }
    }
}
