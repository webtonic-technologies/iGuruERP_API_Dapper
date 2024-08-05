using Admission_API.DTOs.Requests;
using Admission_API.DTOs.ServiceResponse;
using Admission_API.Models;
using Admission_API.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Admission_API.Repository.Implementations
{
    public class RegistrationSetupRepository : IRegistrationSetupRepository
    {
        private readonly IDbConnection _dbConnection;

        public RegistrationSetupRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateRegistrationSetup(RegistrationSetup request)
        {
            var query = request.RegistrationSetupID == 0 ?
                @"INSERT INTO tblRegistrationSetup (FieldName, FieldTypeID, IsDefault) VALUES (@FieldName, @FieldTypeID, @IsDefault)" :
                @"UPDATE tblRegistrationSetup SET FieldName = @FieldName, FieldTypeID = @FieldTypeID, IsDefault = @IsDefault WHERE RegistrationSetupID = @RegistrationSetupID";

            var parameters = new
            {
                RegistrationSetupID = request.RegistrationSetupID,
                FieldName = request.FieldName,
                FieldTypeID = request.FieldTypeID,
                IsDefault = request.IsDefault
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<RegistrationSetup>>> GetAllRegistrationSetups(GetAllRequest request)
        {
            var query = @"SELECT RegistrationSetupID, FieldName, FieldTypeID, IsDefault 
                          FROM tblRegistrationSetup 
                          ORDER BY RegistrationSetupID 
                          OFFSET @PageNumber ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";
            var result = (await _dbConnection.QueryAsync<RegistrationSetup>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblRegistrationSetup");
            return new ServiceResponse<List<RegistrationSetup>>(true, "Operation Successful", result, 200, totalCount);
        }

        public async Task<ServiceResponse<bool>> DeleteRegistrationSetup(int registrationSetupID)
        {
            var query = "DELETE FROM tblRegistrationSetup WHERE RegistrationSetupID = @RegistrationSetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { RegistrationSetupID = registrationSetupID });
            return new ServiceResponse<bool>(true, "Operation Successful", result > 0, 200);
        }
    }
}
