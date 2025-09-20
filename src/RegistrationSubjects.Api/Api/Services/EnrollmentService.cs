using Microsoft.EntityFrameworkCore;
using RegistrationSubjects.Core.Interfaces;
using RegistrationSubjects.Core.Entities;
using RegistrationSubjects.Infrastructure.Data;
using RegistrationSubjects.Core.DTOs;

namespace RegistrationSubjects.Api.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(AppDbContext db, ILogger<EnrollmentService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<SubjectDto>> GetAvailableSubjectsAsync()
        {
            _logger.LogInformation("Iniciando la obtencion de materias disponibles  ------------ ");

                try
                {
                    var a = await _db.Subjects
                    .Include(s => s.Professor)
                    .Select(s => new SubjectDto(
                        s.Id,
                        s.Title,
                        s.Professor.Name
                    ))
                    .ToListAsync();

                    _logger.LogInformation("Materias disponibles {StudentId} ------------ ", a.Count);
                    return a;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener materias");
                    throw;
                }
        }

        public async Task<IEnumerable<StudentInfoDTO>> GetStudentEnrollmentsAsync(int studentId)
        {

            _logger.LogInformation("Iniciando la obtencion de materias inscritas {StudentId} ------------ ", studentId);
            try {
                return await _db.Enrollments
                .Include(e => e.Subject)
                    .ThenInclude(s => s!.Professor!)
                .Where(e => e.StudentId == studentId)
                .Select(e => new StudentInfoDTO(
                    e.StudentId,
                    e.Subject!.Title,
                    e.Subject.Professor.Name
                ))
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias insicritas, estudiante: {StudentId} ------------ ", studentId);
                 throw;
             }

        }

        public async Task<IEnumerable<SubjectWithClassmatesDTO>> GetSharedStudentsAsync(int studentId)
        {

            _logger.LogInformation("Iniciando la obtencion de materias  compartidas con otros estudiantes, estudiante {StudentId} ------------ ", studentId);
            try{
                // materias x estudiante
                var subjectIds = await _db.Enrollments
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.SubjectId)
                    .ToListAsync();

                // traer compañeros
                var result = await _db.Enrollments
                    .Where(e => subjectIds.Contains(e.SubjectId))
                    .Include(e => e.Subject)
                    .Include(e => e.Student)
                    .GroupBy(e => e.Subject!.Title)
                    .Select(g => new SubjectWithClassmatesDTO(
                        g.Key,
                        g
                            .Where(e => e.StudentId != studentId)
                            .Select(e => e.Student!.First_Name)
                            .Distinct()
                            .ToList()
                    ))
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener materias insicritas con otros estudiantes, estudiante: {StudentId} ------------ ", studentId);
                 throw;
            }
        }

        public async Task EnrollAsync(int studentId, List<int> subjectIds)
        {
            _logger.LogInformation("Iniciando la insicripcion de materias, estudiante {StudentId} ------------ ", studentId);
            try{
                //max 3 materias
                if (subjectIds == null || subjectIds.Count == 0)
                    throw new ArgumentException("Debe seleccionar al menos una materia.");

                if (subjectIds.Count > 3)
                    throw new InvalidOperationException("Máximo 3 materias por estudiante.");

                //existencia estudiante
                var student = await _db.Students.FindAsync(studentId);
                if (student == null) throw new KeyNotFoundException("Estudiante no encontrado.");

                // cargar materiasinscritas
                var subjects = await _db.Subjects.Where(s => subjectIds.Contains(s.Id)).ToListAsync();
                if (subjects.Count != subjectIds.Count) throw new KeyNotFoundException("Una o más materias solicitadas no existen.");

                var existingEnrolls = await _db.Enrollments
                    .Include(e => e.Subject)
                    .Where(e => e.StudentId == studentId)
                    .ToListAsync();

                // no más de una materia x profesor
                var existingProfIds = existingEnrolls.Where(e => e.Subject != null).Select(e => e.Subject!.Professor_Id).ToList();
                var newProfIds = subjects.Select(s => s.Professor_Id).ToList();

                var allProfIds = existingProfIds.Concat(newProfIds).ToList();
                if (allProfIds.GroupBy(x => x).Any(g => g.Count() > 1))
                    throw new InvalidOperationException("No puedes tener más de una materia con el mismo profesor.");

                //no duplicados
                foreach (var subj in subjects)
                {
                    var already = existingEnrolls.Any(e => e.SubjectId == subj.Id);
                    if (already) continue;
                    _db.Enrollments.Add(new Enrollment
                    {
                        StudentId = studentId,
                        SubjectId = subj.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iscribir materias, estudiante: {StudentId} ------------ ", studentId);
                 throw;
            }
        }
    }
}
