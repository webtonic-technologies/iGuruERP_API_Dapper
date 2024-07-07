﻿using Transport_API.DTOs.Requests;
using Transport_API.DTOs.ServiceResponse;
using Transport_API.Models;

namespace Transport_API.Repository.Interfaces
{
    public interface IRoutePlanRepository
    {
        Task<ServiceResponse<string>> AddUpdateRoutePlan(RoutePlan routePlan);
        Task<ServiceResponse<IEnumerable<RoutePlan>>> GetAllRoutePlans(GetAllRoutePlanRequest request);
        Task<ServiceResponse<RoutePlan>> GetRoutePlanById(int routePlanId);
        Task<ServiceResponse<bool>> UpdateRoutePlanStatus(int routePlanId);
    }
}
