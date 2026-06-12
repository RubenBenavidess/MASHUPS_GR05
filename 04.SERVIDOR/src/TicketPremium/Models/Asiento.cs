using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("Asientos")]
public class Asiento
{
    [Key]
    [MaxLength(20)]
    public string Codigo { get; set; } = string.Empty;

    [MaxLength(5)]
    public string Fila { get; set; } = string.Empty;

    public int Numero { get; set; }

    [MaxLength(20)]
    public string Estado { get; set; } = "LIBRE";

    public DateTime? TimestampReserva { get; set; }

    [Required]
    [MaxLength(10)]
    public string LocalidadCodigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string PartidoCodigo { get; set; } = string.Empty;

    [ForeignKey(nameof(LocalidadCodigo))]
    public Localidad Localidad { get; set; } = null!;

    [ForeignKey(nameof(PartidoCodigo))]
    public Partido Partido { get; set; } = null!;
}
