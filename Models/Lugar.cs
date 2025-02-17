using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Lugar
{
    [Key]
    [Column("id_lugar")]
    public int IdLugar { get; set; }

    [Required]
    [StringLength(100)]
    [Column("nombre")]

    public string Nombre { get; set; }


    [Required]

    [Column("descripcion")]
    public string Descripcion { get; set; }


    [Required]
    [Column("ubicacion")]

    public string Ubicacion { get; set; }

    [Required]
    [Column("categoria")]

    public string Categoria { get; set; }

    // Relaciones
    public ICollection<Viaje_Lugar> Viaje_Lugares { get; set; } = new List<Viaje_Lugar>();

    
}
