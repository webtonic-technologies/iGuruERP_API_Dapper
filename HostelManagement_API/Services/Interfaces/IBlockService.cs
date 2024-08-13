using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Interfaces
{
    public interface IBlockService
    {
        Task<int> AddUpdateBlocks(AddUpdateBlocksRequest request);
        Task<ServiceResponse<PagedResponse<BlockResponse>>> GetAllBlocks(GetAllBlocksRequest request);
        Task<IEnumerable<BlockResponse>> GetAllBlocksFetch();
        Task<ServiceResponse<BlockResponse>> GetBlockById(int blockId);
        Task<ServiceResponse<int>> DeleteBlock(int blockId);
    }
}
