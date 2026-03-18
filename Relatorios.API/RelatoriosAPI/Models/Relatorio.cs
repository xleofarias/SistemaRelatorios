using System.ComponentModel.DataAnnotations.Schema;

namespace RelatoriosAPI.Models
{
    [Table("Relatorios")]
    public class Relatorio
    {
        public Guid Id { get; set; }
        public required string Tipo { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
