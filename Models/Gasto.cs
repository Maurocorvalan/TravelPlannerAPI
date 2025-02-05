using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Gasto
{
    [Key]
    public int IdGasto { get; set; }

    [Required]
    public int IdViaje { get; set; }

    [Required]
    public string Categoria { get; set; }

    [Required]
    public decimal Monto { get; set; }

    public string Descripcion { get; set; }

    [Required]
    public DateTime FechaGasto { get; set; }

    // Relaciones
    [ForeignKey("IdViaje")]
    public Viaje Viaje { get; set; }
}
