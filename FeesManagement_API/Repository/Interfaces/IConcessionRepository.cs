﻿using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;

namespace FeesManagement_API.Repository.Interfaces
{
    public interface IConcessionRepository
    {
        Task<int> AddUpdateConcession(AddUpdateConcessionRequest request);
        Task<IEnumerable<ConcessionResponse>> GetAllConcessions(GetAllConcessionRequest request);
        Task<ConcessionResponse> GetConcessionById(int concessionGroupID);
        Task<int> UpdateConcessionGroupStatus(int concessionGroupID);
    }
}