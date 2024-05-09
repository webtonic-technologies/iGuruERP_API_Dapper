using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IGalleryService
    {
        Task<ServiceResponse<int>> AddGalleryImage(GalleryDTO galleryDTO);
        Task<ServiceResponse<List<GalleryEventDTO>>> GetApprovedImagesByEvent();
        Task<ServiceResponse<bool>> UpdateGalleryImageApprovalStatus(int galleryId, bool isApproved, int userId);
        Task<ServiceResponse<List<GalleryEventDTO>>> GetAllGalleryImagesByEvent();
    }
}
