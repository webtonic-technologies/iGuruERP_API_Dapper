using HostelManagement_API.DTOs.Requests;
using HostelManagement_API.DTOs.Responses;
using HostelManagement_API.DTOs.ServiceResponse;
using HostelManagement_API.Repository.Interfaces;
using HostelManagement_API.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace HostelManagement_API.Services.Implementations
{
    public class HostelService : IHostelService
    {
        private readonly IHostelRepository _hostelRepository;

        public HostelService(IHostelRepository hostelRepository)
        {
            _hostelRepository = hostelRepository;
        }

        public async Task<ServiceResponse<int>> AddUpdateHostel(AddUpdateHostelRequest request)
        {
            if (!string.IsNullOrEmpty(request.Attachments))
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "attachments");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Attachments;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(request.Attachments));

                request.Attachments = filePath;
            }

            var hostelId = await _hostelRepository.AddUpdateHostel(request);
            return new ServiceResponse<int>(true, "Hostel added/updated successfully", hostelId, 200);
        }

        public async Task<ServiceResponse<PagedResponse<HostelResponse>>> GetAllHostels(GetAllHostelsRequest request)
        {
            var hostels = await _hostelRepository.GetAllHostels(request);
            return new ServiceResponse<PagedResponse<HostelResponse>>(true, "Hostels retrieved successfully", hostels, 200);
        }

        public async Task<ServiceResponse<HostelResponse>> GetHostelById(int hostelId)
        {
            var hostel = await _hostelRepository.GetHostelById(hostelId);
            if (hostel == null)
            {
                return new ServiceResponse<HostelResponse>(false, "Hostel not found", null, 404);
            }
            return new ServiceResponse<HostelResponse>(true, "Hostel retrieved successfully", hostel, 200);
        }

        public async Task<ServiceResponse<bool>> DeleteHostel(int hostelId)
        {
            var result = await _hostelRepository.DeleteHostel(hostelId);
            return new ServiceResponse<bool>(result > 0, result > 0 ? "Hostel deleted successfully" : "Hostel not found", result > 0, result > 0 ? 200 : 404);
        }
    }
}
