using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Gasto
{
    [Key]
    [Column("id_gasto")]
    public int IdGasto { get; set; }

    [Required]
    [Column("id_viaje")]
    public int IdViaje { get; set; }

    [Required]
    [Column("categoria")]
    public string Categoria { get; set; }

    [Required]
    [Column("monto")]
    public decimal Monto { get; set; }

    [Column("descripcion")]
    public string Descripcion { get; set; }

    [Required]
    [Column("fecha_gasto", TypeName  ="DateTime")]
    public DateTime FechaGasto { get; set; }


}
