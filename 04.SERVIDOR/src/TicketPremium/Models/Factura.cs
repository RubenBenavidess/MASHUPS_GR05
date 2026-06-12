using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("Facturas")]
public class Factura
{
    [Key]
    [MaxLength(30)]
    public string Numero { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Descuento { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Iva { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }

    [MaxLength(30)]
    public string MetodoPago { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ClienteCedula { get; set; } = string.Empty;

    [ForeignKey(nameof(ClienteCedula))]
    public Cliente Cliente { get; set; } = null!;

    public ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
}
