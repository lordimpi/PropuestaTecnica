using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PropuestaTecnica.Common.DTOs.Comments;

public class CommentCreateDTO
{
    [Required(ErrorMessage = "El mensaje del comentario es obligatorio.")]
    [MaxLength(1000, ErrorMessage = "El comentario no puede exceder los {1} caracteres.")]
    public string Message { get; set; } = string.Empty;

    //[Required(ErrorMessage = "Debe especificar el usuario que realiza el comentario.")]
    [JsonIgnore]
    public string? UserId { get; set; } = null!;
}
