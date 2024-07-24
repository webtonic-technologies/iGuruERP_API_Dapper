using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostelManagement_API.Controllers
{
    [Route("iGuru/Configuration/[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        private readonly IBlockService _blockService;
        private readonly ILogger<BlockController> _logger;

        public BlockController(IBlockService blockService, ILogger<BlockController> logger)
        {
            _blockService = blockService;
            _logger = logger;
        }

        [HttpPost("AddUpdateBlocks")]
        public async Task<IActionResult> AddUpdateBlock([FromBody] AddUpdateBlockRequest request)
        {
            _logger.LogInformation("AddUpdateBlock Request Received: {@Request}", request);
            var response = await _blockService.AddUpdateBlock(request);
            _logger.LogInformation("AddUpdateBlock Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("GetAllBlocks")]
        public async Task<IActionResult> GetAllBlocks([FromBody] GetAllBlocksRequest request)
        {
            _logger.LogInformation("GetAllBlocks Request Received: {@Request}", request);
            var response = await _blockService.GetAllBlocks(request);
            _logger.LogInformation("GetAllBlocks Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetBlock/{blockId}")]
        public async Task<IActionResult> GetBlock(int blockId)
        {
            _logger.LogInformation("GetBlock Request Received for BlockID: {BlockID}", blockId);
            var response = await _blockService.GetBlockById(blockId);
            _logger.LogInformation("GetBlock Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("Delete/{blockId}")]
        public async Task<IActionResult> DeleteBlock(int blockId)
        {
            _logger.LogInformation("DeleteBlock Request Received for BlockID: {BlockID}", blockId);
            var response = await _blockService.DeleteBlock(blockId);
            _logger.LogInformation("DeleteBlock Response: {@Response}", response);
            return StatusCode(response.StatusCode, response);
        }
    }
}
