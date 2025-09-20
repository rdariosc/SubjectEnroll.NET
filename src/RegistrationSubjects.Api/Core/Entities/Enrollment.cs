namespace RegistrationSubjects.Core.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public  Student? Student { get; set; }

        public int SubjectId { get; set; }
        public  Subject? Subject { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
