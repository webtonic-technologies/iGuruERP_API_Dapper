using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using HostelManagement_API.DTOs.ServiceResponse;

namespace HostelManagement_API.Repository.Interfaces
{
    public interface IBlockRepository
    {
        Task<ServiceResponse<string>> AddUpdateBlocks(AddUpdateBlocksRequest request); 
        Task<ServiceResponse<IEnumerable<BlockResponse>>> GetAllBlocks(GetAllBlocksRequest request);
        Task<IEnumerable<BlockResponse>> GetAllBlocksFetch();
        Task<BlockResponse> GetBlockById(int blockId);
        Task<int> DeleteBlock(int blockId);
    }
}
