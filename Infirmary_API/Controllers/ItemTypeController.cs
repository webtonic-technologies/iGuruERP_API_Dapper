using Infirmary_API.DTOs.Requests;
using Infirmary_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Infirmary_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class ItemTypeController : ControllerBase
    {
        private readonly IItemTypeService _itemTypeService;

        public ItemTypeController(IItemTypeService itemTypeService)
        {
            _itemTypeService = itemTypeService;
        }

        [HttpPost("AddUpdateItemType")]
        public async Task<IActionResult> AddUpdateItemType(AddUpdateItemTypeRequest request)
        {
            try
            {
                var data = await _itemTypeService.AddUpdateItemType(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("GetAllItemTypes")]
        public async Task<IActionResult> GetAllItemTypes(GetAllItemTypesRequest request)
        {
            try
            {
                var data = await _itemTypeService.GetAllItemTypes(request);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet("GetItemTypeById/{ItemTypeID}")]
        public async Task<IActionResult> GetItemTypeById(int ItemTypeID)
        {
            try
            {
                var data = await _itemTypeService.GetItemTypeById(ItemTypeID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPut("DeleteItemType/{ItemTypeID}")]
        public async Task<IActionResult> DeleteItemType(int ItemTypeID)
        {
            try
            {
                var data = await _itemTypeService.DeleteItemType(ItemTypeID);
                if (data.Success)
                {
                    return Ok(data);
                }
                else
                {
                    return BadRequest(data.Message);
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("ItemType/GetItemTypesExport")]
        public async Task<IActionResult> GetItemTypesExport([FromBody] GetItemTypesExportRequest request)
        {
            var response = await _itemTypeService.ExportItemTypesData(request);

            if (response.Success)
            {
                string fileName = "ItemTypes_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + (request.ExportType == 1 ? ".xlsx" : ".csv");
                string contentType = request.ExportType == 1 ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "text/csv";

                return File(response.Data, contentType, fileName);
            }

            return BadRequest(response.Message);
        }
    }
}
