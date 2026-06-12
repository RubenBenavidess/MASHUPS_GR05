using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("DetallesFactura")]
public class DetalleFactura
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string FacturaNumero { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string AsientoCodigo { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioUnitario { get; set; }

    [ForeignKey(nameof(FacturaNumero))]
    public Factura Factura { get; set; } = null!;

    [ForeignKey(nameof(AsientoCodigo))]
    public Asiento Asiento { get; set; } = null!;
}
