namespace RegistrationSubjects.Core.Entities
{
    public class Professor
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required ICollection<Subject> Subjects { get; set; }
    }
}
