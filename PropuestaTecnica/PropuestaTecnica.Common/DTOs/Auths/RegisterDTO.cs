using System.ComponentModel.DataAnnotations;

namespace PropuestaTecnica.Common.DTOs.Auths;

public class RegisterDTO
{
    [EmailAddress(ErrorMessage = "El campo {0} no es una dirección valida.")]
    [Required(ErrorMessage = "El campo {0} es de carácter obligatório.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es de carácter obligatório.")]
    [MinLength(6, ErrorMessage = "El campo {0} debe tener al menos {1} caracteres.")]
    public string Password { get; set; } = string.Empty;
}