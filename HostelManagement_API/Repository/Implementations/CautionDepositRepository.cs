using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace HostelManagement_API.Repository.Implementations
{
    public class CautionDepositRepository : ICautionDepositRepository
    {
        private readonly string _connectionString;

        public CautionDepositRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> AddUpdateCautionDeposit(AddUpdateCautionDepositRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        string sqlQuery = request.CautionFeeID == 0
                            ? @"INSERT INTO tblCautionFee (AcademicYear, FeeMasterName, HostelCautionFee, FeeFrequencyID, InstituteID, IsActive) 
                                VALUES (@AcademicYear, @FeeMasterName, @HostelCautionFee, @FeeFrequencyID, @InstituteID, @IsActive); 
                                SELECT CAST(SCOPE_IDENTITY() as int)"
                            : @"UPDATE tblCautionFee 
                                SET AcademicYear = @AcademicYear, FeeMasterName = @FeeMasterName, HostelCautionFee = @HostelCautionFee, 
                                    FeeFrequencyID = @FeeFrequencyID, InstituteID = @InstituteID, IsActive = @IsActive
                                WHERE CautionFeeID = @CautionFeeID";

                        var cautionFeeId = request.CautionFeeID == 0
                            ? await db.ExecuteScalarAsync<int>(sqlQuery, new { request.AcademicYear, request.FeeMasterName, request.HostelCautionFee, request.FeeFrequencyID, request.InstituteID, request.IsActive }, transaction)
                            : await db.ExecuteAsync(sqlQuery, new { request.AcademicYear, request.FeeMasterName, request.HostelCautionFee, request.FeeFrequencyID, request.InstituteID, request.IsActive, request.CautionFeeID }, transaction);

                        if (request.CautionFeeID == 0)
                        {
                            cautionFeeId = cautionFeeId;
                        }
                        else
                        {
                            cautionFeeId = (int)request.CautionFeeID;
                        }

                        transaction.Commit();
                        return cautionFeeId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<PagedResponse<CautionDepositResponse>> GetAllCautionDeposits(GetAllCautionDepositRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string countQuery = @"SELECT COUNT(*) FROM tblCautionFee WHERE InstituteID = @InstituteID";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                string sqlQuery = @"
                    SELECT 
                        CautionFeeID, AcademicYear, FeeMasterName, HostelCautionFee, FeeFrequencyID, InstituteID, IsActive
                    FROM tblCautionFee
                    WHERE InstituteID = @InstituteID
                    ORDER BY AcademicYear
                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                var cautionDeposits = await db.QueryAsync<CautionDepositResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new PagedResponse<CautionDepositResponse>(cautionDeposits, request.PageNumber, request.PageSize, totalCount);
            }
        }

        public async Task<CautionDepositResponse> GetCautionDepositById(int cautionFeeId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        CautionFeeID, AcademicYear, FeeMasterName, HostelCautionFee, FeeFrequencyID, InstituteID, IsActive
                    FROM tblCautionFee
                    WHERE CautionFeeID = @CautionFeeID";

                return await db.QueryFirstOrDefaultAsync<CautionDepositResponse>(sqlQuery, new { CautionFeeID = cautionFeeId });
            }
        }

        public async Task<int> DeleteCautionDeposit(int cautionFeeId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblCautionFee SET IsActive = 0 WHERE CautionFeeID = @CautionFeeID";
                return await db.ExecuteAsync(sqlQuery, new { CautionFeeID = cautionFeeId });
            }
        }
    }
}
