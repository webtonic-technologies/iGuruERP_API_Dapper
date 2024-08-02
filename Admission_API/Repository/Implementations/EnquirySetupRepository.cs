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
    public class EnquirySetupRepository : IEnquirySetupRepository
    {
        private readonly IDbConnection _dbConnection;

        public EnquirySetupRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<string>> AddUpdateEnquirySetup(EnquirySetup request)
        {
            var query = request.EnquirySetupID == 0 ?
                @"INSERT INTO tblEnquirySetup (FieldName, FieldTypeID, IsDefault, IsDeleted) VALUES (@FieldName, @FieldTypeID, @IsDefault, @IsDeleted)" :
                @"UPDATE tblEnquirySetup SET FieldName = @FieldName, FieldTypeID = @FieldTypeID, IsDefault = @IsDefault, IsDeleted = @IsDeleted WHERE EnquirySetupID = @EnquirySetupID";

            var parameters = new
            {
                EnquirySetupID = request.EnquirySetupID,
                FieldName = request.FieldName,
                FieldTypeID = request.FieldTypeID,
                IsDefault = request.IsDefault,
                IsDeleted = request.IsDeleted
            };

            var result = await _dbConnection.ExecuteAsync(query, parameters);
            return new ServiceResponse<string>(true, "Operation Successful", result.ToString(), 200);
        }

        public async Task<ServiceResponse<List<EnquirySetup>>> GetAllEnquirySetups(GetAllRequest request)
        {
            var query = @"SELECT EnquirySetupID, FieldName, FieldTypeID, IsDefault, IsDeleted 
                          FROM tblEnquirySetup 
                          ORDER BY EnquirySetupID 
                          OFFSET @PageNumber ROWS 
                          FETCH NEXT @PageSize ROWS ONLY";
            var result = (await _dbConnection.QueryAsync<EnquirySetup>(query, new { PageNumber = (request.PageNumber - 1) * request.PageSize, request.PageSize })).ToList();
            var totalCount = await _dbConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tblEnquirySetup");
            return new ServiceResponse<List<EnquirySetup>>(true, "Operation Successful", result, 200, totalCount);
        }

        public async Task<ServiceResponse<bool>> DeleteEnquirySetup(int enquirySetupID)
        {
            var query = "DELETE FROM tblEnquirySetup WHERE EnquirySetupID = @EnquirySetupID";
            var result = await _dbConnection.ExecuteAsync(query, new { EnquirySetupID = enquirySetupID });
            return new ServiceResponse<bool>(true, "Operation Successful", result > 0, 200);
        }
    }
}
