using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text;

namespace FeesManagement_API.Services.Implementations
{
    public class StudentFeeService : IStudentFeeService
    {
        private readonly IStudentFeeRepository _studentFeeRepository;

        public StudentFeeService(IStudentFeeRepository studentFeeRepository)
        {
            _studentFeeRepository = studentFeeRepository;
        }
         

        public ServiceResponse<List<StudentFeeResponse>> GetStudentFees(StudentFeeRequest request)
        {
            var studentFees = _studentFeeRepository.GetStudentFees(request);

            var totalCount = studentFees?.Count ?? 0; // Calculate total count if needed

            var response = new ServiceResponse<List<StudentFeeResponse>>(
                success: true,
                message: "Student fees retrieved successfully",
                data: studentFees,
                statusCode: 200,
                totalCount: totalCount
            );

            return response;
        }

        public ServiceResponse<DiscountStudentFeesResponse> DiscountStudentFees(DiscountStudentFeesRequest request)
        {
            // Insert discount record and get the DiscountID
            var discountID = _studentFeeRepository.DiscountStudentFees(request);

            var responseDto = new DiscountStudentFeesResponse
            {
                DiscountID = discountID,
                Message = "Discount applied successfully."
            };

            return new ServiceResponse<DiscountStudentFeesResponse>(
                success: true,
                message: "Discount applied successfully",
                data: responseDto,
                statusCode: 200
            );
        }

        public ServiceResponse<List<GetFeesChangeLogsResponse>> GetFeesChangeLogs(GetFeesChangeLogsRequest request)
        {
            var logs = _studentFeeRepository.GetFeesChangeLogs(request);
            var totalCount = logs?.Count ?? 0;
            return new ServiceResponse<List<GetFeesChangeLogsResponse>>(
                success: true,
                message: "Fees change logs retrieved successfully",
                data: logs,
                statusCode: 200,
                totalCount: totalCount
            );
        }




        public async Task<ServiceResponse<List<StudentFeeResponse>>> GetStudentFeesExport(GetStudentFeesExportRequest request)
        {
            try
            {
                var data = await _studentFeeRepository.GetStudentFeesExport(request);

                if (data == null || data.Count == 0)
                {
                    return new ServiceResponse<List<StudentFeeResponse>>(
                        success: false,
                        message: "No records found",
                        data: null,
                        statusCode: 404
                    );
                }

                return new ServiceResponse<List<StudentFeeResponse>>(
                    success: true,
                    message: "Student fee data retrieved successfully",
                    data: data,
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<StudentFeeResponse>>(
                    success: false,
                    message: $"Error: {ex.Message}",
                    data: null,
                    statusCode: 500
                );
            }
        }

    }
}
