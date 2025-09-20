using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RegistrationSubjects.Api;
using RegistrationSubjects.Core.DTOs;
using Xunit;
using Xunit.Abstractions;


namespace RegistrationSubjects.IntegrationTests
{
   public class EnrollmentIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public EnrollmentIntegrationTests(CustomWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _client = factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task GetSubjects_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/enrollments/subjects");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Enroll_ReturnsCreated()
        {
            var dto = new EnrollDTO(1, new List<int> { 1 });
            var response = await _client.PostAsJsonAsync("/api/enrollments", dto);
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine("Respuesta del servidor:");
            _output.WriteLine(content);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetStudentEnrolls_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/enrollments/student/1");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetShared_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/enrollments/shared/1");
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
