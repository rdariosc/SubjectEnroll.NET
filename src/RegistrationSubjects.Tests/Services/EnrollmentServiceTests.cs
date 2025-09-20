using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RegistrationSubjects.Core.DTOs;
using RegistrationSubjects.Core.Entities;
using RegistrationSubjects.Infrastructure.Data;
using RegistrationSubjects.Api.Services;
using Xunit;

namespace RegistrationSubjects.Tests.Services
{
    public class EnrollmentServiceTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        private EnrollmentService GetService(AppDbContext context)
        {
            var mockLogger = new Mock<ILogger<EnrollmentService>>();
            return new EnrollmentService(context, mockLogger.Object);
        }

        [Fact]
        public async Task GetAvailableSubjectsAsync_ReturnsEmpty_WhenNoSubjects()
        {
            var db = GetDbContext("db1");
            var service = GetService(db);

            var result = await service.GetAvailableSubjectsAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAvailableSubjectsAsync_ReturnsSubjects_WithProfessors()
        {
            var db = GetDbContext("db2");
            var professor = new Professor { Id = 1, Name = "John", Subjects = new List<Subject>() };
            var subject = new Subject { Id = 10, Title = "Matematicas", Professor_Id = 1, Professor = professor, Enrollments = new List<Enrollment>() };
            professor.Subjects.Add(subject);

            db.Professors.Add(professor);
            db.Subjects.Add(subject);
            await db.SaveChangesAsync();

            var service = GetService(db);

            var result = await service.GetAvailableSubjectsAsync();

            var subjectDto = Assert.Single(result);
            Assert.Equal("Matematicas", subjectDto.Title);
            Assert.Equal("John", subjectDto.ProfessorName);
        }

        [Fact]
        public async Task GetStudentEnrollmentsAsync_ReturnsEmpty_WhenNoEnrollments()
        {
            var db = GetDbContext("db3");
            var service = GetService(db);

            var result = await service.GetStudentEnrollmentsAsync(1);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStudentEnrollmentsAsync_ReturnsEnrollments()
        {
            var db = GetDbContext("db4");
            var professor = new Professor { Id = 1, Name = "Juan", Subjects = new List<Subject>() };
            var subject = new Subject { Id = 20, Title = "Fisica", Professor_Id = 1, Professor = professor, Enrollments = new List<Enrollment>() };
            professor.Subjects.Add(subject);

            db.Subjects.Add(subject);
            db.Enrollments.Add(new Enrollment { Id = 100, StudentId = 1, SubjectId = 20, Subject = subject });
            await db.SaveChangesAsync();

            var service = GetService(db);

            var result = await service.GetStudentEnrollmentsAsync(1);

            var enrollment = Assert.Single(result);
            Assert.Equal("Fisica", enrollment.SubjectTitle);
            Assert.Equal("Juan", enrollment.ProfessorName);
        }

        [Fact]
        public async Task GetSharedStudentsAsync_ReturnsClassmates()
        {
            var db = GetDbContext("db5");

            var prof = new Professor { Id = 1, Name = "John", Subjects = new List<Subject>() };
            var subject = new Subject { Id = 10, Title = "Matematicas", Professor_Id = 1, Professor = prof, Enrollments = new List<Enrollment>() };
            prof.Subjects.Add(subject);

            var student1 = new Student { Id = 1, First_Name = "carlos", Last_Name = "rod" };
            var student2 = new Student { Id = 2, First_Name = "juan", Last_Name = "mendoza" };
            var student3 = new Student { Id = 3, First_Name = "camila", Last_Name = "sanchez" };

            db.Students.AddRange(student1, student2, student3);
            db.Subjects.Add(subject);
            db.Enrollments.AddRange(
                new Enrollment { StudentId = 1, SubjectId = 10, Subject = subject, Student = student1 },
                new Enrollment { StudentId = 2, SubjectId = 10, Subject = subject, Student = student2 },
                new Enrollment { StudentId = 3, SubjectId = 10, Subject = subject, Student = student3 }
            );
            await db.SaveChangesAsync();

            var service = GetService(db);

            var result = await service.GetSharedStudentsAsync(1);

            var math = Assert.Single(result);
            Assert.Equal("Matematicas", math.SubjectTitle);
            Assert.Contains("juan", math.Classmates);
            Assert.Contains("camila", math.Classmates);
            Assert.DoesNotContain("carlos", math.Classmates);
        }

        [Fact]
        public async Task EnrollAsync_Throws_WhenNoSubjects()
        {
            var db = GetDbContext("db6");
            var service = GetService(db);

            await Assert.ThrowsAsync<ArgumentException>(() => service.EnrollAsync(1, new List<int>()));
        }

        [Fact]
        public async Task EnrollAsync_Throws_WhenMoreThan3Subjects()
        {
            var db = GetDbContext("db7");
            var service = GetService(db);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.EnrollAsync(1, new List<int> { 1, 2, 3, 4 }));
        }

        [Fact]
        public async Task EnrollAsync_Throws_WhenStudentNotFound()
        {
            var db = GetDbContext("db8");
            var service = GetService(db);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.EnrollAsync(99, new List<int> { 1 }));
        }

        [Fact]
        public async Task EnrollAsync_Throws_WhenSubjectNotFound()
        {
            var db = GetDbContext("db9");
            db.Students.Add(new Student { Id = 1, First_Name = "Test", Last_Name = "User" });
            await db.SaveChangesAsync();

            var service = GetService(db);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.EnrollAsync(1, new List<int> { 123 }));
        }

        [Fact]
        public async Task EnrollAsync_Throws_WhenSameProfessorAlreadyEnrolled()
        {
            var db = GetDbContext("db10");

            var prof = new Professor { Id = 1, Name = "Juan", Subjects = new List<Subject>() };
            var subj1 = new Subject { Id = 1, Title = "Matematicas", Professor_Id = 1, Professor = prof, Enrollments = new List<Enrollment>() };
            var subj2 = new Subject { Id = 2, Title = "Fisica", Professor_Id = 1, Professor = prof, Enrollments = new List<Enrollment>() };
            prof.Subjects = new List<Subject> { subj1, subj2 };

            var student = new Student { Id = 1, First_Name = "Ana", Last_Name = "Lopez" };

            db.Students.Add(student);
            db.Subjects.AddRange(subj1, subj2);
            db.Enrollments.Add(new Enrollment { StudentId = 1, SubjectId = 1, Subject = subj1 });
            await db.SaveChangesAsync();

            var service = GetService(db);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.EnrollAsync(1, new List<int> { 2 }));
        }

        [Fact]
        public async Task EnrollAsync_Success_WhenValid()
        {
            var db = GetDbContext("db11");

            var prof1 = new Professor { Id = 1, Name = "Andres", Subjects = new List<Subject>() };
            var prof2 = new Professor { Id = 2, Name = "juan", Subjects = new List<Subject>() };

            var subj1 = new Subject { Id = 1, Title = "Matematicas", Professor_Id = 1, Professor = prof1, Enrollments = new List<Enrollment>() };
            var subj2 = new Subject { Id = 2, Title = "Fisica", Professor_Id = 2, Professor = prof2, Enrollments = new List<Enrollment>() };
            prof1.Subjects.Add(subj1);
            prof2.Subjects.Add(subj2);

            var student = new Student { Id = 1, First_Name = "Ana", Last_Name = "Lopez" };

            db.Students.Add(student);
            db.Subjects.AddRange(subj1, subj2);
            await db.SaveChangesAsync();

            var service = GetService(db);

            await service.EnrollAsync(1, new List<int> { 1, 2 });

            var enrolls = db.Enrollments.Where(e => e.StudentId == 1).ToList();
            Assert.Equal(2, enrolls.Count);
            Assert.Contains(enrolls, e => e.SubjectId == 1);
            Assert.Contains(enrolls, e => e.SubjectId == 2);
        }
    }
}
