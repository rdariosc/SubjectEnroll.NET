namespace RegistrationSubjects.Core.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int Credits { get; set; }
        
        public int Professor_Id { get; set; }
        public required Professor Professor { get; set; }

        public required ICollection<Enrollment> Enrollments { get; set; }
    }
}
