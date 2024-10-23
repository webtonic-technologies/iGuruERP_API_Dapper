using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FeesManagement_API.Repository.Implementations
{
    public class FeeStructureRepository : IFeeStructureRepository
    {
        private readonly IConfiguration _configuration;

        public FeeStructureRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FeeStructureResponse GetFeeStructure(FeeStructureRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open(); // Ensure the connection is open

                var query = @"SELECT 
                         fh.FeeHead AS FeeHead,
                         CASE 
                             WHEN fg.FeeTenurityID = 1 THEN 'Single'
                             WHEN fg.FeeTenurityID = 2 THEN tt.TermName
                             WHEN fg.FeeTenurityID = 3 THEN tm.Month
                         END AS TenureType,
                         COALESCE(ts.Amount, tt.Amount, tm.Amount) AS Amount
                     FROM tblFeeGroup fg
                     INNER JOIN tblFeeHead fh ON fg.FeeHeadID = fh.FeeHeadID
                     INNER JOIN tblFeeGroupCollection fgc ON fg.FeeGroupID = fgc.FeeGroupID
                     INNER JOIN tblFeeGroupClassSection fgcs ON fg.FeeGroupID = fgcs.FeeGroupID
                     LEFT JOIN tblTenuritySingle ts ON fgc.FeeCollectionID = ts.FeeCollectionID
                     LEFT JOIN tblTenurityTerm tt ON fgc.FeeCollectionID = tt.FeeCollectionID
                     LEFT JOIN tblTenurityMonthly tm ON fgc.FeeCollectionID = tm.FeeCollectionID
                     WHERE fgcs.ClassID = @ClassID 
                       AND fgcs.SectionID = @SectionID
                       AND fg.InstituteID = @InstituteID
                     ORDER BY fh.FeeHead;";

                var feeStructures = connection.Query<FeeDetail>(query, new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID
                }).ToList();

                // Calculate the total fees
                var totalFees = feeStructures.Sum(f => f.Amount);

                // Return the response
                return new FeeStructureResponse
                {
                    FeeDetails = feeStructures,
                    TotalFees = totalFees
                };
            }
        }

    }
}
