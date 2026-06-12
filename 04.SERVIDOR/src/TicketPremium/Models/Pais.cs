using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.TicketPremium.Models;

[Table("Paises")]
public class Pais
{
    [Key]
    [MaxLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Continente { get; set; } = string.Empty;

    public ICollection<Estadio> Estadios { get; set; } = new List<Estadio>();
}
