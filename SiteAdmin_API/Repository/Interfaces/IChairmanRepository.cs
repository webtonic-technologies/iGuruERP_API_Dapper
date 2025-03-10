﻿using System.Threading.Tasks;
using SiteAdmin_API.DTOs.Requests;
using SiteAdmin_API.DTOs.Responses;
using SiteAdmin_API.DTOs.ServiceResponse;

namespace SiteAdmin_API.Repository.Interfaces
{
    public interface IChairmanRepository
    {
        Task<ServiceResponse<string>> AddUpdateChairman(AddUpdateChairmanRequest request);
        Task<ServiceResponse<IEnumerable<GetAllChairmanResponse>>> GetAllChairman();
        Task<ServiceResponse<string>> DeleteChairman(int chairmanID);
        Task<ServiceResponse<List<GetInstitutesDDLResponse>>> GetInstitutesDDL();
        Task<ServiceResponse<CreateUserResponse>> CreateUserLoginInfo(CreateUserRequest request);
    }
}
