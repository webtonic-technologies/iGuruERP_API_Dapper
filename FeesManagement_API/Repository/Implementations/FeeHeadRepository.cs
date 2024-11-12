using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Response;
using FeesManagement_API.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FeesManagement_API.DTOs.ServiceResponse;


namespace FeesManagement_API.Repository.Implementations
{
    public class FeeHeadRepository : IFeeHeadRepository
    {
        private readonly IDbConnection _connection;

        public FeeHeadRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> AddUpdateFeeHead(AddUpdateFeeHeadRequest request)
        {
            int rowsAffected = 0;
            foreach (var feeHead in request.FeeHeads)
            {
                var query = feeHead.FeeHeadID == 0
                    ? @"INSERT INTO tblFeeHead (FeeHead, ShortName, RegTypeID, IsActive, InstituteID) 
                        VALUES (@FeeHeadName, @ShortName, @RegTypeID, @IsActive, @InstituteID);
                        SELECT CAST(SCOPE_IDENTITY() as int);"
                    : @"UPDATE tblFeeHead 
                        SET FeeHead = @FeeHeadName, 
                            ShortName = @ShortName, 
                            RegTypeID = @RegTypeID, 
                            IsActive = @IsActive,
                            InstituteID = @InstituteID
                        WHERE FeeHeadID = @FeeHeadID";

                if (feeHead.FeeHeadID == 0)
                {
                    var insertedId = await _connection.ExecuteScalarAsync<int>(query, feeHead);
                    if (insertedId > 0)
                    {
                        rowsAffected++;
                    }
                }
                else
                {
                    var affectedRows = await _connection.ExecuteAsync(query, feeHead);
                    if (affectedRows > 0)
                    {
                        rowsAffected++;
                    }
                }
            }
            return rowsAffected;
        }

        public async Task<ServiceResponse<IEnumerable<FeeHeadResponse>>> GetAllFeeHead(GetAllFeeHeadRequest request)
        {
            // Query to get the total count of fee heads
            var countQuery = @"SELECT COUNT(*) 
                       FROM tblFeeHead fh
                       WHERE fh.InstituteID = @InstituteID AND fh.IsActive = 1";

            var totalCount = await _connection.ExecuteScalarAsync<int>(countQuery, new { request.InstituteID });

            // Main query to get the paginated fee heads
            var query = @"SELECT fh.FeeHeadID, 
                 fh.FeeHead AS FeeHeadName, 
                 fh.ShortName, 
                 fh.RegTypeID, 
                 rt.RegType, 
                 fh.IsActive, 
                 fh.InstituteID 
          FROM tblFeeHead fh
          INNER JOIN tblFeeHeadingRegType rt ON fh.RegTypeID = rt.RegTypeID
          WHERE fh.InstituteID = @InstituteID AND fh.IsActive = 1
          ORDER BY fh.FeeHeadID
          OFFSET @Offset ROWS
          FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                request.InstituteID
            };

            var feeHeads = await _connection.QueryAsync<FeeHeadResponse>(query, parameters);

            return new ServiceResponse<IEnumerable<FeeHeadResponse>>(true, "FeeHeads retrieved successfully", feeHeads, 200, totalCount);
        }

        public async Task<ServiceResponse<IEnumerable<FeeHeadResponse>>> GetAllFeeHeadDDL(GetAllFeeHeadDDLRequest request)
        {
            // Main query to get all active fee heads
            var query = @"SELECT fh.FeeHeadID, 
                  fh.FeeHead AS FeeHeadName, 
                  fh.ShortName, 
                  fh.RegTypeID, 
                  rt.RegType, 
                  fh.IsActive, 
                  fh.InstituteID 
                  FROM tblFeeHead fh
                  INNER JOIN tblFeeHeadingRegType rt ON fh.RegTypeID = rt.RegTypeID
                  WHERE fh.InstituteID = @InstituteID AND fh.IsActive = 1
                  ORDER BY fh.FeeHeadID";

            var feeHeads = await _connection.QueryAsync<FeeHeadResponse>(query, new { request.InstituteID });

            return new ServiceResponse<IEnumerable<FeeHeadResponse>>(true, "FeeHeads retrieved successfully", feeHeads, 200);
        }



        //public async Task<IEnumerable<FeeHeadResponse>> GetAllFeeHead(GetAllFeeHeadRequest request)
        //{
        //    var query = @"SELECT fh.FeeHeadID, 
        //                 fh.FeeHead AS FeeHeadName, 
        //                 fh.ShortName, 
        //                 fh.RegTypeID, 
        //                 rt.RegType, 
        //                 fh.IsActive, 
        //                 fh.InstituteID 
        //          FROM tblFeeHead fh
        //          INNER JOIN tblFeeHeadingRegType rt ON fh.RegTypeID = rt.RegTypeID
        //          WHERE fh.InstituteID = @InstituteID and fh.IsActive = 1
        //          ORDER BY fh.FeeHeadID
        //          OFFSET @Offset ROWS
        //          FETCH NEXT @PageSize ROWS ONLY";

        //    var parameters = new
        //    {
        //        Offset = (request.PageNumber - 1) * request.PageSize,
        //        request.PageSize,
        //        request.InstituteID
        //    };

        //    return await _connection.QueryAsync<FeeHeadResponse>(query, parameters);
        //}


        public async Task<FeeHeadResponse> GetFeeHeadById(int feeHeadId)
        {
            var query = @"
        SELECT 
            fh.FeeHeadID, 
            fh.FeeHead AS FeeHeadName, 
            fh.ShortName, 
            fh.RegTypeID, 
            rt.RegType, -- Fetch the RegType name
            fh.IsActive, 
            fh.InstituteID 
        FROM 
            tblFeeHead fh
        LEFT JOIN 
            tblFeeHeadingRegType rt ON fh.RegTypeID = rt.RegTypeID
        WHERE  
            fh.FeeHeadID = @FeeHeadID AND  fh.IsActive = 1";

            return await _connection.QueryFirstOrDefaultAsync<FeeHeadResponse>(query, new { FeeHeadID = feeHeadId });
        }


        public async Task<int> DeleteFeeHead(int feeHeadId)
        {
            var query = @"UPDATE tblFeeHead 
                          SET IsActive = 0 
                          WHERE FeeHeadID = @FeeHeadID";

            return await _connection.ExecuteAsync(query, new { FeeHeadID = feeHeadId });
        }
    }
}
