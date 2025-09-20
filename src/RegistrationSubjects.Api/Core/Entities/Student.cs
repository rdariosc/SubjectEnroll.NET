namespace RegistrationSubjects.Core.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public required string First_Name { get; set; }
        public required string Last_Name { get; set; }
        public  string? Email { get; set; }
        public  string? Password { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
