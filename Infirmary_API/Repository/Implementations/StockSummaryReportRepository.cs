using Dapper;
using Infirmary_API.DTOs.Requests;
using Infirmary_API.DTOs.Responses;
using Infirmary_API.Repository.Interfaces;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using Infirmary_API.DTOs.ServiceResponse;

namespace Infirmary_API.Repository.Implementations
{
    public class StockSummaryReportRepository : IStockSummaryReportRepository
    {
        private readonly IDbConnection _connection;

        public StockSummaryReportRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ServiceResponse<List<GetStockSummaryReportResponse>>> GetStockSummaryReport(GetStockSummaryReportRequest request)
        {
            try
            {
                string query = @"
            SELECT 
                it.ItemType AS ItemType,
                se.MedicineName AS ItemName, 
                se.Company, 
                sq.BatchCode,
                sq.Quantity AS OpeningStock,
                COALESCE(SUM(m.Quantity), 0) AS InQuantity,
                (sq.Quantity - COALESCE(SUM(m.Quantity), 0)) AS OutQuantity,
                (sq.Quantity - (sq.Quantity - COALESCE(SUM(m.Quantity), 0))) AS ClosingStock
            FROM 
                tblStockEntry se
            INNER JOIN 
                tblStockQuantity sq ON se.StockID = sq.StockID
            LEFT JOIN 
                tblMedicine m ON sq.StockID = m.PrescribedMedicineID
            INNER JOIN 
                tblInfirmaryItemType it ON se.ItemTypeID = it.ItemTypeID
            WHERE 
                se.InstituteID = @InstituteID 
                AND se.EntryDate BETWEEN @StartDate AND @EndDate
            GROUP BY 
                se.ItemTypeID, it.ItemType, se.MedicineName, se.Company, sq.BatchCode, sq.Quantity
            ORDER BY 
                se.ItemTypeID, se.MedicineName
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY;
        ";

                // Calculate OFFSET based on PageNumber and PageSize
                int offset = (request.PageNumber - 1) * request.PageSize;

                var result = await _connection.QueryAsync<GetStockSummaryReportResponse>(query, new
                {
                    request.InstituteID,
                    StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                    Offset = offset,
                    PageSize = request.PageSize
                });

                return new ServiceResponse<List<GetStockSummaryReportResponse>>(true, "Stock summary report generated successfully", result.ToList(), 200, result.Count());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetStockSummaryReportResponse>>(false, ex.Message, null, 500);
            }
        }

        public async Task<List<GetStockSummaryReportExportResponse>> GetStockSummaryReportExport(GetStockSummaryReportExportRequest request)
        {
            string query = @"
                SELECT 
                    it.ItemType AS ItemType,
                    se.MedicineName AS ItemName, 
                    se.Company, 
                    sq.BatchCode,
                    sq.Quantity AS OpeningStock,
                    COALESCE(SUM(m.Quantity), 0) AS InQuantity,
                    (sq.Quantity - COALESCE(SUM(m.Quantity), 0)) AS OutQuantity,
                    (sq.Quantity - (sq.Quantity - COALESCE(SUM(m.Quantity), 0))) AS ClosingStock
                FROM 
                    tblStockEntry se
                INNER JOIN 
                    tblStockQuantity sq ON se.StockID = sq.StockID
                LEFT JOIN 
                    tblMedicine m ON sq.StockID = m.PrescribedMedicineID
                INNER JOIN 
                    tblInfirmaryItemType it ON se.ItemTypeID = it.ItemTypeID
                WHERE 
                    se.InstituteID = @InstituteID 
                    AND se.EntryDate BETWEEN @StartDate AND @EndDate
                GROUP BY 
                    se.ItemTypeID, it.ItemType, se.MedicineName, se.Company, sq.BatchCode, sq.Quantity
                ORDER BY 
                    se.ItemTypeID, se.MedicineName";

            var result = await _connection.QueryAsync<GetStockSummaryReportExportResponse>(query, new
            {
                request.InstituteID,
                StartDate = DateTime.ParseExact(request.StartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                EndDate = DateTime.ParseExact(request.EndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)
            });

            return result.ToList();
        }
    }
}
