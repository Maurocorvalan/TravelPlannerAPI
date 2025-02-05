using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Lugar
{
    [Key]
    public int IdLugar { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    [Required]
    public string Ubicacion { get; set; }

    [Required]
    public string Categoria { get; set; }

    // Relaciones
    public List<Viaje_Lugar> Viajes { get; set; }
}
