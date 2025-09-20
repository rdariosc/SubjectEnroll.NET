using RegistrationSubjects.Core.DTOs;

namespace RegistrationSubjects.Core.Interfaces
{
    public interface IEnrollmentService
    {
        Task EnrollAsync(int studentId, List<int> subjectIds);
        Task<IEnumerable<SubjectDto>> GetAvailableSubjectsAsync();
        Task<IEnumerable<StudentInfoDTO>> GetStudentEnrollmentsAsync(int studentId);
        Task<IEnumerable<SubjectWithClassmatesDTO>> GetSharedStudentsAsync(int studentId);
    }
}
