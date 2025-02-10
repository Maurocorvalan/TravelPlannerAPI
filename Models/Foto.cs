using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Foto
{
    [Key]
    [Column("id_foto")]

    public int IdFoto { get; set; }

    [Required]
    [Column("id_viaje")]

    public int IdViaje { get; set; }

    [Required]
    [StringLength(255)]
    public string Url { get; set; }

    public string Descripcion { get; set; }
    [Column("fecha_subida")]
    public DateTime FechaSubida { get; set; } = DateTime.Now;

    // Relaciones
    [ForeignKey("IdViaje")]
    public Viaje Viaje { get; set; }
}
