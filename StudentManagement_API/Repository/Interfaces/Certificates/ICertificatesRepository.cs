﻿using StudentManagement_API.DTOs.Requests;
using StudentManagement_API.DTOs.Responses;
using StudentManagement_API.DTOs.ServiceResponse;


namespace StudentManagement_API.Repository.Interfaces
{
    public interface ICertificatesRepository
    {
        Task<int> CreateCertificateTemplateAsync(CreateCertificateTemplateRequest request);
        //Task<IEnumerable<GetCertificateTemplateResponse>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request);
        //Task<int> GenerateCertificateAsync(GenerateCertificateRequest request);

        Task<ServiceResponse<IEnumerable<GetCertificateTemplateResponse>>> GetCertificateTemplateAsync(GetCertificateTemplateRequest request);

        //Task<List<int>> GenerateCertificatesAsync(GenerateCertificateRequest request);
        Task<GenerateCertificateResponse> GenerateCertificatesAsync(GenerateCertificateRequest request);


        //Task<IEnumerable<GetStudentsResponse>> GetStudentsAsync(GetStudentsRequest request);
        Task<ServiceResponse<IEnumerable<GetStudentsResponse>>> GetStudentsAsync(GetStudentsRequest request);
        //Task<IEnumerable<GetCertificateReportResponse>> GetCertificateReportAsync(GetCertificateReportRequest request);
        Task<ServiceResponse<IEnumerable<GetCertificateReportResponse>>> GetCertificateReportAsync(GetCertificateReportRequest request);
        Task<IEnumerable<GetCertificateReportExportResponse>> GetCertificateReportExportAsync(GetCertificateReportExportRequest request);
        Task<IEnumerable<GetCertificateInstituteTagsResponse>> GetCertificateInstituteTagsAsync();
        Task<IEnumerable<CertificateStudentTagDto>> GetCertificateStudentTagsAsync();
        Task<ServiceResponse<GetCertificateTagValueResponse>> GetCertificateTagValue(GetCertificateTagValueRequest request);
        Task<ServiceResponse<int>> AttachCertificatewithStudent(AttachCertificatewithStudentRequest request);

    }
}
 