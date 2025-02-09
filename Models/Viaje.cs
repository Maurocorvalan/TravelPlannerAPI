using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class Viaje
{
    [Key]
    [Column("id_viaje")]

    public int IdViaje { get; set; }

    [Required]
    [Column("id_usuario")]

    public int IdUsuario { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    [Required]
    [Column("fecha_inicio")]

    public DateTime FechaInicio { get; set; }

    [Required]
    [Column("fecha_fin")]

    public DateTime FechaFin { get; set; }
    // Relaciones
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }

    public List<Gasto> Gastos { get; set; }
    public List<Foto> Fotos { get; set; }
    public List<Viaje_Lugar> Lugares { get; set; }
}
