using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("Estadios")]
public class Estadio
{
    [Key]
    [MaxLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Ciudad { get; set; } = string.Empty;

    public int CapacidadTotal { get; set; }

    [Required]
    [MaxLength(10)]
    public string PaisCodigo { get; set; } = string.Empty;

    [ForeignKey(nameof(PaisCodigo))]
    public Pais Pais { get; set; } = null!;

    public ICollection<Localidad> Localidades { get; set; } = new List<Localidad>();
    public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
}
