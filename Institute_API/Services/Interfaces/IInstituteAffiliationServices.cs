﻿using Institute_API.DTOs.ServiceResponse;
using Institute_API.DTOs;

namespace Institute_API.Services.Interfaces
{
    public interface IInstituteAffiliationServices
    {
        Task<ServiceResponse<int>> AddUpdateInstituteAffiliation(AffiliationDTO request);
        Task<ServiceResponse<AffiliationDTO>> GetAffiliationInfoById(int Id);
    }
}
