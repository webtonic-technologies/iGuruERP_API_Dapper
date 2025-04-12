using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace FeesManagement_API.Controllers
{
    [ApiController]
    [Route("iGuru/FeeCollection/PaymentChecklist")]
    public class PaymentChecklistController : ControllerBase
    {
        private readonly IPaymentChecklistService _paymentChecklistService;

        public PaymentChecklistController(IPaymentChecklistService paymentChecklistService)
        {
            _paymentChecklistService = paymentChecklistService;
        }

        [HttpPost("Set")]
        public async Task<IActionResult> Set([FromBody] List<PaymentChecklistSetRequest> requests)
        {
            if (requests == null || !requests.Any())
            {
                return BadRequest("Invalid request: At least one checklist item is required.");
            }

            var response = await _paymentChecklistService.SetPaymentChecklist(requests);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPost("Get")]
        public IActionResult Get([FromBody] PaymentChecklistGetRequest request)
        {
            var response = _paymentChecklistService.GetPaymentChecklist(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
