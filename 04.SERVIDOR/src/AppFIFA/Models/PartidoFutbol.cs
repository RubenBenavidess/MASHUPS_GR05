using System.ComponentModel.DataAnnotations;

namespace ec.edu.monster.AppFIFA.Models;

public class PartidoFutbol
{
    [Key]
    public string Codigo { get; set; } = string.Empty;

    public string EquipoLocal { get; set; } = string.Empty;

    public string EquipoVisitante { get; set; } = string.Empty;

    public DateTime FechaHora { get; set; }

    public string EstadioCodigo { get; set; } = string.Empty;

    public ICollection<LocalidadPartido> Localidades { get; set; } = new List<LocalidadPartido>();
}
