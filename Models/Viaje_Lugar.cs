using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Viaje_Lugar
{
    [Key]
    [Column("id_viaje")]
    public int IdViaje { get; set; }

    [Key]
    [Column("id_lugar")]
    public int IdLugar { get; set; }
}