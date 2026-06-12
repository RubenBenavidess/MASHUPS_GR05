using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.CoreBancario.Models
{
    public class Amortizacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public int CreditoCodigo { get; set; }

        [Required]
        public int NumeroCuota { get; set; }

        [Required]
        public decimal ValorCuota { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [ForeignKey(nameof(CreditoCodigo))]
        public Credito Credito { get; set; } = null!;
    }
}
