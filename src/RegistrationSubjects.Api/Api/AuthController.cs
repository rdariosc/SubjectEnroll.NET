using Microsoft.AspNetCore.Mvc;
using RegistrationSubjects.Infrastructure.Data;
using RegistrationSubjects.Core.Entities;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JWTService _tokenService;

    public AuthController(AppDbContext db, JWTService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {

        if (!ModelState.IsValid)
        return BadRequest(new { message = "Datos invalidos." });

        if (await _db.Students.AnyAsync(s => s.Email == dto.Email))
            return Conflict(new { message = "Email ya  existe." });

        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var student = new Student { First_Name = dto.FirstName, Last_Name = dto.LastName, Email = dto.Email, Password = hash };
        _db.Students.Add(student);
        await _db.SaveChangesAsync();

        var token = _tokenService.GenerateToken(student.Id, student.Email);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var student = await _db.Students.SingleOrDefaultAsync(s => s.Email == dto.Email);
        if (student == null || !BCrypt.Net.BCrypt.Verify(dto.Password, student.Password))
            return Unauthorized(new { message = "Cedenciales Invalidas." });

        var token = _tokenService.GenerateToken(student.Id, student.Email!);
        return Ok(new { token, studentId = student.Id });
    }
}

public record LoginDto(string Email, string Password);
