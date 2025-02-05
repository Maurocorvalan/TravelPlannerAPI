using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Viaje
{
    [Key]
    public int IdViaje { get; set; }

    [Required]
    public int IdUsuario { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    // Relaciones
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }

    public List<Itinerario> Itinerarios { get; set; }
    public List<Gasto> Gastos { get; set; }
    public List<Foto> Fotos { get; set; }
    public List<Viaje_Lugar> Lugares { get; set; }
}
