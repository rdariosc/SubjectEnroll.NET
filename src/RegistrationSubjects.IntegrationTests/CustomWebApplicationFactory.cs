using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RegistrationSubjects.Infrastructure.Data;
using RegistrationSubjects.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace RegistrationSubjects.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                var professor = new Professor
                {
                    Id = 1,
                    Name = "Andres",
                    Subjects = new List<Subject>()
                };

                var subject = new Subject
                {
                    Id = 1,
                    Title = "Matematicas",
                    Professor = professor,
                    Enrollments = new List<Enrollment>()
                };

                var student = new Student
                {
                    Id = 1,
                    First_Name = "Juan",
                    Last_Name = "Pérez"
                };

                professor.Subjects.Add(subject);

                db.Students.Add(student);
                db.Professors.Add(professor);
                db.Subjects.Add(subject);

                //apagar para porbar inscripcion de materias
                //db.Enrollments.Add(new Enrollment { StudentId = 1, Subject = subject, CreatedAt = DateTime.UtcNow });

                db.SaveChanges();
            });
        }
    }
}
