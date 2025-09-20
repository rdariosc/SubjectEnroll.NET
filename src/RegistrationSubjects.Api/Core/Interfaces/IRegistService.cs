using RegistrationSubjects.Core.Entities;

namespace RegistrationSubjects.Core.Interfaces
{
    public interface IRegistService
    {
        Task EnrollAsync(int studentId, List<int> subjectIds);
        Task<List<Subject>> GetStudentSubjects(int studentId);
        Task<List<string>> GetSharedStudentNames(int subjectId, int studentId);
    }
}