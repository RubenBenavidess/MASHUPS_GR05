using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("Clientes")]
public class Cliente
{
    [Key]
    [MaxLength(20)]
    public string Cedula { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Apellido { get; set; } = string.Empty;

    public DateTime FechaNacimiento { get; set; }

    [MaxLength(20)]
    public string Genero { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Telefono { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Rol { get; set; } = "CLIENTE";

    public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}
