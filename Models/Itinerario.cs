using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Itinerario
{
    [Key]
    public int IdItinerario { get; set; }

    [Required]
    public int IdViaje { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    [StringLength(255)]
    public string Actividad { get; set; }

    public string Ubicacion { get; set; }

    // Relaciones
    [ForeignKey("IdViaje")]
    public Viaje Viaje { get; set; }
}
