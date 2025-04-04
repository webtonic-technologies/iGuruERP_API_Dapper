using Microsoft.AspNetCore.Mvc;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/FeeCollection/Refund")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpPost("AddRefund")]
        public ActionResult<ServiceResponse<string>> AddRefund([FromBody] AddRefundRequest request)
        {
            var result = _refundService.AddRefund(request);
            return Ok(new ServiceResponse<string>(true, "Refund added successfully", result, 200));
        }

        //[HttpPost("GetRefund")]
        //public ActionResult<ServiceResponse<List<GetRefundResponse>>> GetRefund([FromBody] GetRefundRequest request)
        //{
        //    var result = _refundService.GetRefund(request);
        //    return Ok(new ServiceResponse<List<GetRefundResponse>>(true, "Refunds retrieved successfully", result, 200));
        //}

        [HttpPost("GetRefund")]
        public async Task<ActionResult<ServiceResponse<List<GetRefundResponse>>>> GetRefund([FromBody] GetRefundRequest request)
        {
            var serviceResponse = await _refundService.GetRefund(request);
            // Assuming serviceResponse.Data is IEnumerable<GetRefundResponse>, convert it to List
            var refundList = serviceResponse.Data.ToList();
            return Ok(new ServiceResponse<List<GetRefundResponse>>(
                success: true,
                message: "Refunds retrieved successfully",
                data: refundList,
                statusCode: 200,
                totalCount: serviceResponse.TotalCount));
        }


        [HttpDelete("DeleteRefund/{refundID}")]
        public ActionResult<ServiceResponse<string>> DeleteRefund(int refundID)
        {
            var response = _refundService.DeleteRefund(refundID);
            return Ok(response);
        }


        [HttpPost("GetRefundExport")]
        public IActionResult GetRefundExport([FromBody] GetRefundExportRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request: Details are required.");
            }

            try
            {
                var fileData = _refundService.GetRefundExport(request);
                var contentType = request.ExportType == 1
                    ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    : "text/csv";
                var fileName = request.ExportType == 1
                    ? "RefundExport.xlsx"
                    : "RefundExport.csv";

                return File(fileData, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("GetStudentList")]
        public ActionResult<ServiceResponse<IEnumerable<GetStudentListResponse>>> GetStudentList([FromBody] GetStudentListRequest request)
        {
            var studentList = _refundService.GetStudentList(request);
            return Ok(new ServiceResponse<IEnumerable<GetStudentListResponse>>(
                success: true,
                message: "Student list retrieved successfully",
                data: studentList,
                statusCode: 200));
        }

        [HttpPost("GetRefundPaymentMode")]
        public ActionResult<ServiceResponse<IEnumerable<GetRefundPaymentModeResponse>>> GetRefundPaymentMode()
        {
            var paymentModes = _refundService.GetRefundPaymentMode();
            return Ok(new ServiceResponse<IEnumerable<GetRefundPaymentModeResponse>>(
                success: true,
                message: "Payment modes retrieved successfully",
                data: paymentModes,
                statusCode: 200
            ));
        }
    }
}
