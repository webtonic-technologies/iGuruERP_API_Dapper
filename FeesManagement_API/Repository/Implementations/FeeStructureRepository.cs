using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

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

                var query = @"
            SELECT 
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
                AND fg.AcademicYearCode = @AcademicYearCode   -- Added filter for AcademicYearCode
            ORDER BY fh.FeeHead;";

                var feeStructures = connection.Query<FeeDetail>(query, new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID,
                    request.AcademicYearCode  // Passing AcademicYearCode to query
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

        public async Task<byte[]> GetFeeStructureExcel(FeeStructureRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open(); // Ensure the connection is open

                var query = @"
        SELECT 
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
            AND fg.AcademicYearCode = @AcademicYearCode
        ORDER BY fh.FeeHead;";

                var feeStructures = connection.Query<FeeDetail>(query, new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID,
                    request.AcademicYearCode
                }).ToList();


                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Fee Structure");

                    // Set headers for Excel
                    worksheet.Cells[1, 1].Value = "Fee Head";
                    worksheet.Cells[1, 2].Value = "Tenure Type";
                    worksheet.Cells[1, 3].Value = "Amount";

                    var row = 2;
                    foreach (var feeStructure in feeStructures)
                    {
                        worksheet.Cells[row, 1].Value = feeStructure.FeeHead;
                        worksheet.Cells[row, 2].Value = feeStructure.TenureType;
                        worksheet.Cells[row, 3].Value = feeStructure.Amount;
                        row++;
                    }

                    // Auto fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    return package.GetAsByteArray(); // Return the byte array of the Excel file
                }
            }
        }

        public async Task<byte[]> GetFeeStructureCSV(FeeStructureRequest request)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var query = @"
                SELECT 
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
                    AND fg.AcademicYearCode = @AcademicYearCode
                ORDER BY fh.FeeHead;";

                var feeStructures = connection.Query<FeeDetail>(query, new
                {
                    request.ClassID,
                    request.SectionID,
                    request.InstituteID,
                    request.AcademicYearCode
                }).ToList();

                // Generate CSV content
                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                {
                    // Write header
                    writer.WriteLine("Fee Head,Tenure Type,Amount");

                    // Write data
                    foreach (var feeStructure in feeStructures)
                    {
                        writer.WriteLine($"{feeStructure.FeeHead},{feeStructure.TenureType},{feeStructure.Amount}");
                    }

                    writer.Flush();
                    memoryStream.Position = 0;

                    return memoryStream.ToArray(); // Return the CSV as a byte array
                }
            }
        }


    }
}
