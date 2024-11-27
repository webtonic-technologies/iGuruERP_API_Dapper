using Configuration.DTOs.Requests;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeesManagement_API.Controllers
{
    [Route("iGuru/Configuration/Offer")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OfferController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        [HttpPost("AddUpdateOffer")]
        public async Task<IActionResult> AddUpdateOffer([FromBody] AddUpdateOfferRequest request)
        {
            var response = await _offerService.AddUpdateOffer(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPost("GetAllOffers")]
        public async Task<IActionResult> GetAllOffers([FromBody] GetAllOffersRequest request)
        {
            var response = await _offerService.GetAllOffers(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        //[HttpPost("GetAllOffers")]
        //public async Task<IActionResult> GetAllOffers([FromBody] GetAllOffersRequest request)
        //{
        //    var response = await _offerService.GetAllOffers(request);
        //    if (response.Success)
        //    {
        //        return Ok(response);
        //    }
        //    return BadRequest(response);
        //}

        //[HttpGet("GetOfferById/{offerID}")]
        //public async Task<IActionResult> GetOfferById(int offerID)
        //{
        //    var response = await _offerService.GetOfferById(offerID);
        //    if (response.Success)
        //    {
        //        return Ok(response);
        //    }
        //    return NotFound(response);
        //}

        [HttpPut("DeleteOffer/{offerID}")]
        public async Task<IActionResult> DeleteOffer(int offerID)
        {
            var response = await _offerService.DeleteOffer(offerID);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }


        [HttpPost("GetOfferStudentType")]
        public async Task<IActionResult> GetOfferStudentType()
        {
            var result = await _offerService.GetOfferStudentTypes();
            return Ok(new
            {
                success = true,
                message = "Student types retrieved successfully",
                data = result
            });
        }
    }
}
