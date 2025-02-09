using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Itinerario
{
    [Key]
    [Column("id_itinerario")]
    public int IdItinerario { get; set; }

    [Required]
    [Column("id_viaje")]
    public int IdViaje { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    [StringLength(500)]
    public string Actividad { get; set; }

    [StringLength(200)]
    public string Ubicacion { get; set; }

    // Relación con la tabla Viaje
    [ForeignKey("IdViaje")]
    public Viaje Viaje { get; set; }
}
