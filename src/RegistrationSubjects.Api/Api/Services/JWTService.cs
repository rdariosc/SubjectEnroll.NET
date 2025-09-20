using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


public class JWTService
{
    private readonly IConfiguration _config;
    private readonly ILogger<JWTService> _logger;

    public JWTService(IConfiguration config,ILogger<JWTService> logger)
    {
        _config = config;
        _logger = logger;
    }


    public string GenerateToken(int studentId, string email)
    {
        _logger.LogInformation("Iniciando la generacion del Token JWT, estudiante: {StudentId}   ------------ ", studentId);
        try{
                var jwt = _config.GetSection("Jwt");
                var key = Encoding.ASCII.GetBytes(jwt["Key"]!);

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.NameIdentifier, studentId.ToString()),
                        new Claim(ClaimTypes.Email, email)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"]!)),
                    Issuer = jwt["Issuer"],
                    Audience = jwt["Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar toke JWT, estudiante: {StudentId} ------------ ", studentId);
                 throw;
            }
    }
}
