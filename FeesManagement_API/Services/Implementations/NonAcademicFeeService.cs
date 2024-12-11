﻿using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;
using FeesManagement_API.Utilities;

namespace FeesManagement_API.Services.Implementations
{
    public class NonAcademicFeeService : INonAcademicFeeService
    {
        private readonly INonAcademicFeeRepository _repository;

        public NonAcademicFeeService(INonAcademicFeeRepository repository)
        {
            _repository = repository;
        }

        public ServiceResponse<string> AddNonAcademicFee(AddUpdateNonAcademicFeeRequest request)
        {
            var result = _repository.AddNonAcademicFee(request);
            return new ServiceResponse<string>(true, "Successfully added/updated the non-academic fee", result, 200);
        }

        public ServiceResponse<List<GetNonAcademicFeeResponse>> GetNonAcademicFee(GetNonAcademicFeeRequest request)
        {
            var result = _repository.GetNonAcademicFee(request);
            return new ServiceResponse<List<GetNonAcademicFeeResponse>>(true, "Successfully retrieved data", result, 200);
        }
         
        public ServiceResponse<string> DeleteNonAcademicFee(int nonAcademicFeesID)
        {
            var result = _repository.DeleteNonAcademicFee(nonAcademicFeesID);
            return new ServiceResponse<string>(true, "Successfully deleted the non-academic fee", result, 200);
        }

        public byte[] GetNonAcademicFeeExport(GetNonAcademicFeeExportRequest request)
        {
            var dataTable = _repository.GetNonAcademicFeeExportData(request);

            return request.ExportType switch
            {
                1 => FileExportHelper.ExportToExcel(dataTable), // Excel
                2 => FileExportHelper.ExportToCsv(dataTable),   // CSV
                _ => throw new ArgumentException("Invalid ExportType")
            };
        }
    }
}
