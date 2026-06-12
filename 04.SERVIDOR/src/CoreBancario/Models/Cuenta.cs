using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.CoreBancario.Models
{
    public class Cuenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Numero { get; set; } = null!;

        [Required]
        public string ClienteCedula { get; set; } = null!;

        [Required]
        public string Tipo { get; set; } = null!;

        [Required]
        public decimal Saldo { get; set; }

        [ForeignKey(nameof(ClienteCedula))]
        public ClienteBanco ClienteBanco { get; set; } = null!;

        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}
