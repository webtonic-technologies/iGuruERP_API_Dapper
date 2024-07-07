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
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly IDbConnection _dbConnection;

        public RegistrationRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddRegistration(Registration request)
        {
            var query = @"INSERT INTO tblRegistrationMaster (RegistrationGroupID, FieldTypeID, FieldTypeValue) 
                          VALUES (@RegistrationGroupID, @FieldTypeID, @FieldTypeValue)";
            var parameters = new
            {
                request.RegistrationGroupID,
                request.FieldTypeID,
                request.FieldTypeValue
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<Registration>>> GetAllRegistrations(GetAllRequest request)
        {
            var query = @"SELECT RegistrationID, RegistrationGroupID, FieldTypeID, FieldTypeValue 
                          FROM tblRegistrationMaster 
                          ORDER BY RegistrationID 
                          OFFSET @PageNumber ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";
            var result = (await _dbConnection.QueryAsync<Registration>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblRegistrationMaster");
            return new ServiceResponse<List<Registration>>(true, "Operation Successful", result, 200, totalCount);
        }

        public async Task<ServiceResponse<string>> SendRegistrationMessage(SendRegistrationMessageRequest request)
        {
            var query = @"INSERT INTO tblRegistrationSMS (RegistrationID, TemplateID, SMSDetails) 
                          VALUES (@RegistrationID, @TemplateID, @SMSDetails)";
            var parameters = new
            {
                request.RegistrationID,
                request.TemplateID,
                request.SMSDetails
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<RegistrationSMS>>> GetRegistrationSMSReport()
        {
            var query = @"SELECT rs.RegistrationSMSID, rs.RegistrationID, rs.TemplateID, rs.SMSDetails 
                          FROM tblRegistrationSMS rs 
                          JOIN tblRegistrationMaster rm ON rs.RegistrationID = rm.RegistrationID";
            var result = (await _dbConnection.QueryAsync<RegistrationSMS>(query)).ToList();
            return new ServiceResponse<List<RegistrationSMS>>(true, "Operation Successful", result, 200);
        }
    }
}
