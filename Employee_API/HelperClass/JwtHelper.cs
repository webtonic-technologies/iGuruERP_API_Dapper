using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

public class JwtHelper
{
    private readonly IConfiguration _config;
    public JwtHelper(IConfiguration configuration)
    {
        _config = configuration;
    }
    public string GenerateJwtToken(int employeeId, string UserType, string UserName)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Id", employeeId.ToString()),
                new Claim("UserType", UserType),
                new Claim("UserName", UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var jwtToken = new JwtSecurityToken(
       claims: claims,
       notBefore: DateTime.UtcNow,
       expires: DateTime.UtcNow.AddDays(30),
       signingCredentials: new SigningCredentials(
           new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(_config["Jwt:Key"])
               ),
           SecurityAlgorithms.HmacSha256Signature)
       );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}