using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public required  string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    public required  string LastName { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public required  string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public required  string Password { get; set; }
}