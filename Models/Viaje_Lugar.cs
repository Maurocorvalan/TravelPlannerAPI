using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Viaje_Lugar
{
    [Required]
    public int IdViaje { get; set; }

    [Required]
    public int IdLugar { get; set; }

    // Relaciones
    [ForeignKey("IdViaje")]
    public Viaje Viaje { get; set; }

    [ForeignKey("IdLugar")]
    public Lugar Lugar { get; set; }
}
