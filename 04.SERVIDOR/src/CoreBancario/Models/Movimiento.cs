using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.CoreBancario.Models
{
    public class Movimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public string CuentaNumero { get; set; } = null!;

        [Required]
        public string Tipo { get; set; } = null!;

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [ForeignKey(nameof(CuentaNumero))]
        public Cuenta Cuenta { get; set; } = null!;
    }
}
