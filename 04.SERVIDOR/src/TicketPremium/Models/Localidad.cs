using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("Localidades")]
public class Localidad
{
    [Key]
    [MaxLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Descripcion { get; set; } = string.Empty;

    public int Capacidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioBase { get; set; }

    [Required]
    [MaxLength(10)]
    public string EstadioCodigo { get; set; } = string.Empty;

    [ForeignKey(nameof(EstadioCodigo))]
    public Estadio Estadio { get; set; } = null!;

    public ICollection<Asiento> Asientos { get; set; } = new List<Asiento>();
}
