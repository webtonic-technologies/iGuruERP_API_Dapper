using Dapper;
using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;
using Transport_API.Repository.Interfaces;
using System.Data;
using Transport_API.DTOs.Response;

namespace Transport_API.Repository.Implementations
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly IDbConnection _dbConnection;

        public ReportsRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetTransportPendingFeeReport(GetReportsRequest request)
        {
            string sql = @"SELECT ReportType, ReportData FROM tbl_Reports WHERE ReportType = 'TransportPendingFee' ORDER BY ReportId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var reports = await _dbConnection.QueryAsync<ReportResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (reports.Any())
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(true, "Records Found", reports, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetEmployeeTransportationReport(GetReportsRequest request)
        {
            string sql = @"SELECT ReportType, ReportData FROM tbl_Reports WHERE ReportType = 'EmployeeTransportation' ORDER BY ReportId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var reports = await _dbConnection.QueryAsync<ReportResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (reports.Any())
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(true, "Records Found", reports, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetStudentTransportReport(GetReportsRequest request)
        {
            string sql = @"SELECT ReportType, ReportData FROM tbl_Reports WHERE ReportType = 'StudentTransport' ORDER BY ReportId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var reports = await _dbConnection.QueryAsync<ReportResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (reports.Any())
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(true, "Records Found", reports, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }

        public async Task<ServiceResponse<IEnumerable<ReportResponse>>> GetDeAllocationReport(GetReportsRequest request)
        {
            string sql = @"SELECT ReportType, ReportData FROM tbl_Reports WHERE ReportType = 'DeAllocation' ORDER BY ReportId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var reports = await _dbConnection.QueryAsync<ReportResponse>(sql, new { Offset = (request.PageNumber - 1) * request.PageSize, PageSize = request.PageSize });

            if (reports.Any())
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(true, "Records Found", reports, StatusCodes.Status200OK);
            }
            else
            {
                return new ServiceResponse<IEnumerable<ReportResponse>>(false, "No Records Found", null, StatusCodes.Status204NoContent);
            }
        }
    }
}
