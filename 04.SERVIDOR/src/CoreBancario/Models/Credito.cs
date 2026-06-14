using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.CoreBancario.Models
{
    public class Credito
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Codigo { get; set; }

        [Required]
        public Guid ClienteId { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public int PlazoMeses { get; set; }

        [Required]
        public decimal TasaAnual { get; set; }

        [Required]
        public DateTime FechaAprobacion { get; set; }

        [Required]
        public string Estado { get; set; } = null!;

        [ForeignKey(nameof(ClienteId))]
        public ClienteBanco ClienteBanco { get; set; } = null!;

        public ICollection<Amortizacion> Amortizaciones { get; set; } = new List<Amortizacion>();
    }
}
