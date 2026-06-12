using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.AppFIFA.Models;

public class Asiento
{
    [Key]
    public string CodigoAsiento { get; set; } = string.Empty;

    public string Fila { get; set; } = string.Empty;

    public int Numero { get; set; }

    public string Estado { get; set; } = "LIBRE";

    public DateTime? TimestampReserva { get; set; }

    public string LocalidadPartidoCodigo { get; set; } = string.Empty;

    [ForeignKey(nameof(LocalidadPartidoCodigo))]
    public LocalidadPartido LocalidadPartido { get; set; } = null!;
}
