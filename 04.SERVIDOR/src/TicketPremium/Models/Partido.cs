using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("Partidos")]
public class Partido
{
    [Key]
    [MaxLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EquipoLocal { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EquipoVisitante { get; set; } = string.Empty;

    public DateTime FechaHora { get; set; }

    [Required]
    [MaxLength(10)]
    public string EstadioCodigo { get; set; } = string.Empty;

    [ForeignKey(nameof(EstadioCodigo))]
    public Estadio Estadio { get; set; } = null!;

    public ICollection<Asiento> Asientos { get; set; } = new List<Asiento>();
}
