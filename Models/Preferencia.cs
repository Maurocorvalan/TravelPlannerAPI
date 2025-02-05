using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Preferencia
{
    [Key]
    public int IdPreferencia { get; set; }

    [Required]
    public int IdUsuario { get; set; }

    [Required]
    public string Categoria { get; set; }

    // Relaciones
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }
}
