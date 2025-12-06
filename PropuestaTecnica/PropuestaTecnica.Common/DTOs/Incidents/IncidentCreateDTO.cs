using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PropuestaTecnica.Common.DTOs.Incidents;

public class IncidentCreateDTO
{
    [Required(ErrorMessage = "El título del incidente es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder los {1} caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción del incidente es obligatoria.")]
    [MaxLength(2000, ErrorMessage = "La descripción no puede exceder los {1} caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe especificar una categoría válida.")]
    public int CategoryId { get; set; }

    [JsonIgnore]
    public string? UserId { get; set; } = string.Empty;
}
