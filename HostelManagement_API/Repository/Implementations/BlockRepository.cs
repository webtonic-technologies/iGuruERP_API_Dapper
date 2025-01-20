using Dapper;
using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Repository.Implementations
{
    public class BlockRepository : IBlockRepository
    {
        private readonly string _connectionString;

        public BlockRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<ServiceResponse<string>> AddUpdateBlocks(AddUpdateBlocksRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        foreach (var block in request.Blocks)
                        {
                            string sqlQuery = block.BlockID == 0
                                ? @"INSERT INTO tblBlock (BlockName, InstituteID, IsActive) 
                            VALUES (@BlockName, @InstituteID, @IsActive); 
                            SELECT CAST(SCOPE_IDENTITY() as int)"
                                : @"UPDATE tblBlock 
                            SET BlockName = @BlockName, InstituteID = @InstituteID, IsActive = @IsActive
                            WHERE BlockID = @BlockID";

                            var blockId = block.BlockID == 0
                                ? await db.ExecuteScalarAsync<int>(sqlQuery, new { block.BlockName, block.InstituteID, block.IsActive }, transaction)
                                : await db.ExecuteAsync(sqlQuery, new { block.BlockName, block.InstituteID, block.IsActive, block.BlockID }, transaction);
                        }

                        transaction.Commit();

                        return new ServiceResponse<string>(
                            success: true,
                            message: "Block(s) Added/Updated Successfully",
                            data: "Success",
                            statusCode: 200,
                            totalCount: null
                        );
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<ServiceResponse<IEnumerable<BlockResponse>>> GetAllBlocks(GetAllBlocksRequest request)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string countQuery = @"SELECT COUNT(*) FROM tblBlock WHERE InstituteID = @InstituteID AND IsActive = 1";
                int totalCount = await db.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

                string sqlQuery = @"SELECT * FROM tblBlock WHERE InstituteID = @InstituteID AND IsActive = 1
                                    ORDER BY BlockID
                                    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                var blocks = await db.QueryAsync<BlockResponse>(sqlQuery, new
                {
                    request.InstituteID,
                    Offset = (request.PageNumber - 1) * request.PageSize,
                    PageSize = request.PageSize
                });

                return new ServiceResponse<IEnumerable<BlockResponse>>(true, "Blocks Retrieved Successfully", blocks, statusCode: 200, totalCount);
                 
            }
        }
        public async Task<IEnumerable<BlockResponse>> GetAllBlocksFetch()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        BlockID, BlockName, InstituteID, IsActive
                    FROM tblBlock WHERE IsActive = 1
                    ORDER BY BlockName";

                var blocks = await db.QueryAsync<BlockResponse>(sqlQuery);
                return blocks;
            }
        }

        public async Task<BlockResponse> GetBlockById(int blockId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"SELECT * FROM tblBlock WHERE BlockID = @BlockID AND IsActive = 1";
                return await db.QueryFirstOrDefaultAsync<BlockResponse>(sqlQuery, new { BlockID = blockId });
            }
        }

        public async Task<int> DeleteBlock(int blockId)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE tblBlock SET IsActive = 0 WHERE BlockID = @BlockID";
                return await db.ExecuteAsync(sqlQuery, new { BlockID = blockId });
            }
        }
    }
}
