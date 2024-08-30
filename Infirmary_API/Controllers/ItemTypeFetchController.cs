using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class ItemTypeFetchController : ControllerBase
    {
        private readonly IItemTypeFetchService _itemTypeFetchService;

        public ItemTypeFetchController(IItemTypeFetchService itemTypeFetchService)
        {
            _itemTypeFetchService = itemTypeFetchService;
        }

        [HttpPost("GetAllItemTypes_Fetch")]
        public async Task<IActionResult> GetAllItemTypesFetch(GetAllItemTypesFetchRequest request)
        {
            try
            {
                var data = await _itemTypeFetchService.GetAllItemTypesFetch(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return NotFound(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}
