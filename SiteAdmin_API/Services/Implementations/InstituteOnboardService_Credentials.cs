using SiteAdmin_API.DTOs.Response;
using SiteAdmin_API.DTOs.ServiceResponse;
using SiteAdmin_API.Services.Interfaces;
using System;
using System.Linq;
using System.Text;

namespace SiteAdmin_API.Services.Implementations
{
    public class InstituteOnboardService_Credentials : IInstituteOnboardService_Credentials
    {
        public ServiceResponse<GenerateInstituteCredentialsResponse_Credentials> GenerateInstituteCredentials(string instituteName)
        {
            if (string.IsNullOrEmpty(instituteName))
            {
                return new ServiceResponse<GenerateInstituteCredentialsResponse_Credentials>(false, "InstituteName cannot be null or empty", null, 400);
            }

            // Generate username by taking the first letter of each word in the institute name
            var username = new string(instituteName.Split(' ')
                                .Select(word => word[0])
                                .Take(8) // Limit to 8 characters
                                .ToArray())
                                .ToUpper();

            // Generate a 10-character alphanumeric password
            var password = GenerateRandomPassword(10);

            var response = new GenerateInstituteCredentialsResponse_Credentials
            {
                Username = username,
                Password = password
            };

            return new ServiceResponse<GenerateInstituteCredentialsResponse_Credentials>(true, "Credentials generated successfully", response, 200);
        }

        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
