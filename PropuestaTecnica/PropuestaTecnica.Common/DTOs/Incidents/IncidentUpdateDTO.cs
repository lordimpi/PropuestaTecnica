using PropuestaTecnica.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace PropuestaTecnica.Common.DTOs.Incidents;

public class IncidentUpdateDTO
{
    [Required(ErrorMessage = "El título es obligatorio para la actualización.")]
    [MaxLength(200, ErrorMessage = "El título no puede exceder los {1} caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria para la actualización.")]
    [MaxLength(2000, ErrorMessage = "La descripción no puede exceder los {1} caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe especificar un estado válido para el incidente.")]
    public IncidentStatus Status { get; set; }
}
