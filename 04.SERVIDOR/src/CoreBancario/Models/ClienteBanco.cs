using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ec.edu.monster.CoreBancario.Models
{
    public class ClienteBanco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public Guid Id { get; set; }

        [Required]
        public string Cedula { get; set; } = null!;

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        public string Genero { get; set; } = null!;

        [Required]
        public string Estado { get; set; } = null!;

        public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        public ICollection<Credito> Creditos { get; set; } = new List<Credito>();
    }
}
