using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.AppFIFA.Models;

public class LocalidadPartido
{
    [Key]
    public string Codigo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public string PartidoCodigo { get; set; } = string.Empty;

    public int Capacidad { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PrecioBase { get; set; }

    [ForeignKey(nameof(PartidoCodigo))]
    public PartidoFutbol Partido { get; set; } = null!;

    public ICollection<Asiento> Asientos { get; set; } = new List<Asiento>();
}
