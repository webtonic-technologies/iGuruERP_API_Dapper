﻿using LibraryManagement_API.DTOs.Requests;
using LibraryManagement_API.DTOs.ServiceResponses;
using LibraryManagement_API.DTOs.Responses;
using LibraryManagement_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement_API.Repository.Interfaces
{
    public interface ICatalogueRepository
    {
        Task<ServiceResponse<List<CatalogueResponse>>> GetAllCatalogues(GetAllCataloguesRequest request);
        Task<ServiceResponse<Catalogue>> GetCatalogueById(int catalogueId);
        Task<ServiceResponse<string>> AddUpdateCatalogue(Catalogue request);
        Task<ServiceResponse<bool>> DeleteCatalogue(int catalogueId);
    }
}
