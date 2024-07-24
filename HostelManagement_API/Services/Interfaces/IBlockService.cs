using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IBlockService
    {
        Task<ServiceResponse<int>> AddUpdateBlock(AddUpdateBlockRequest request);
        Task<ServiceResponse<PagedResponse<BlockResponse>>> GetAllBlocks(GetAllBlocksRequest request);
        Task<ServiceResponse<BlockResponse>> GetBlockById(int blockId);
        Task<ServiceResponse<int>> DeleteBlock(int blockId);
    }
}
