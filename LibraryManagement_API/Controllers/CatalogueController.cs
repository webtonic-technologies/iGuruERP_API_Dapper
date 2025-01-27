using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.Models;
using LibraryManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LibraryManagement_API.Controllers.Operations
{
    [Route("iGuru/Operations/[controller]")]
    [ApiController]
    public class CatalogueController : ControllerBase
    {
        private readonly ICatalogueService _catalogueService;

        public CatalogueController(ICatalogueService catalogueService)
        {
            _catalogueService = catalogueService;
        }

        [HttpPost("GetAllCatalogue")]
        public async Task<IActionResult> GetAllCatalogues(GetAllCataloguesRequest request)
        {
            var response = await _catalogueService.GetAllCatalogues(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("AddUpdateCatalogue")]
        public async Task<IActionResult> AddUpdateCatalogue(Catalogue request)
        {
            var response = await _catalogueService.AddUpdateCatalogue(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetCatalogue/{catalogueId}")]
        public async Task<IActionResult> GetCatalogueById(int catalogueId)
        {
            var response = await _catalogueService.GetCatalogueById(catalogueId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{catalogueId}")]
        public async Task<IActionResult> DeleteCatalogue(int catalogueId)
        {
            var response = await _catalogueService.DeleteCatalogue(catalogueId);
            return StatusCode(response.StatusCode, response);
        }



        [HttpPost("GetCatalogueSetting")]
        public async Task<IActionResult> GetCatalogueSetting([FromBody] GetCatalogueSettingRequest request)
        {
            try
            {
                var response = await _catalogueService.GetCatalogueSetting(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("AddRemoveCatalogueSetting")]
        public async Task<IActionResult> AddRemoveCatalogueSetting([FromBody] AddRemoveCatalogueSettingRequest request)
        {
            try
            {
                var response = await _catalogueService.AddRemoveCatalogueSetting(request);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
