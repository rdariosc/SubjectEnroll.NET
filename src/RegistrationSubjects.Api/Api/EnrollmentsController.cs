using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistrationSubjects.Core.Interfaces;
using RegistrationSubjects.Core.DTOs;

namespace RegistrationSubjects.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _svc;
        public EnrollmentsController(IEnrollmentService svc) => _svc = svc;

        [Authorize]
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects()
        {

            var subs = await _svc.GetAvailableSubjectsAsync();

            var dtoList = subs.Select(s => new SubjectDto(
                s.SubjectId,
                s.Title,
                s.ProfessorName
            ));
            var temp = dtoList;
            return Ok(temp);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Enroll([FromBody] EnrollDTO req)
        {
                await _svc.EnrollAsync(req.StudentId, req.SubjectIds);
                return Created("", null);
        }

        [Authorize]
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentEnrolls(int studentId)
        {
            var enroll = await _svc.GetStudentEnrollmentsAsync(studentId);
            return Ok(enroll);
        }

        [Authorize]
        [HttpGet("shared/{studentId}")]
        public async Task<IActionResult> GetShared(int studentId)
        {
            var classmates = await _svc.GetSharedStudentsAsync(studentId);
            return Ok(classmates);
        }
    }
}
