using FeesManagement_API.DTOs.Requests;
using FeesManagement_API.DTOs.Responses;
using FeesManagement_API.DTOs.ServiceResponse;
using FeesManagement_API.Repository.Implementations;
using FeesManagement_API.Repository.Interfaces;
using FeesManagement_API.Services.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace FeesManagement_API.Services.Implementations
{
    public class ConcessionMappingService : IConcessionMappingService
    {
        private readonly IConcessionMappingRepository _repository;

        public ConcessionMappingService(IConcessionMappingRepository repository)
        {
            _repository = repository;
        }

        public ServiceResponse<string> AddUpdateConcession(AddUpdateConcessionMappingRequest request)
        {
            var result = _repository.AddUpdateConcession(request);
            return new ServiceResponse<string>(true, "Concession added/updated successfully", result, 200);
        }

        public ServiceResponse<List<GetAllConcessionMappingResponse>> GetAllConcessionMapping(GetAllConcessionMappingRequest request)
        {
            var result = _repository.GetAllConcessionMapping(request);
            return new ServiceResponse<List<GetAllConcessionMappingResponse>>(true, "Concessions retrieved successfully", result, 200, result.Count);
        }

        public async Task<ServiceResponse<byte[]>> GetConcessionListExcel(GetAllConcessionMappingRequest request)
        {
            var fileBytes = _repository.GetConcessionListExcel(request);  // Get the byte array from the repository
            return new ServiceResponse<byte[]>(true, "Excel file created successfully", fileBytes, 200);  // Return the byte array wrapped in a ServiceResponse
        }

        public async Task<ServiceResponse<byte[]>> GetConcessionListCsv(GetAllConcessionMappingRequest request)
        {
            // Retrieve the concession list from the repository
            var concessionList = await _repository.GetConcessionListForExport(request);

            // Generate the CSV content
            var csvContent = new StringBuilder();
            csvContent.AppendLine("StudentID,StudentName,ClassName,SectionName,AdmissionNumber,ConcessionGroupType,IsActive");

            foreach (var concession in concessionList)
            {
                csvContent.AppendLine($"{concession.StudentID},{concession.StudentName},{concession.ClassName},{concession.SectionName},{concession.AdmissionNumber},{concession.ConcessionGroupType},{(concession.IsActive ? "Yes" : "No")}");
            }

            // Convert the string to byte array for download
            byte[] fileBytes = Encoding.UTF8.GetBytes(csvContent.ToString());

            return new ServiceResponse<byte[]>(true, "CSV file created successfully", fileBytes, 200);
        }



        public ServiceResponse<string> UpdateStatus(int studentConcessionID)
        {
            var result = _repository.UpdateStatus(studentConcessionID);
            return new ServiceResponse<string>(true, "Status updated successfully", result, 200);
        }
        public async Task<ServiceResponse<IEnumerable<ConcessionListResponse>>> GetConcessionList(ConcessionListRequest request)
        {
            var concessionList = await _repository.GetConcessionList(request.InstituteID);

            return new ServiceResponse<IEnumerable<ConcessionListResponse>>(
                true,
                "Concession groups retrieved successfully",
                concessionList,
                200
            );
        }
    }
}
