using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Foto
{
    [Key]
    public int IdFoto { get; set; }

    [Required]
    public int IdViaje { get; set; }

    [Required]
    public string Url { get; set; }

    public string Descripcion { get; set; }

    public DateTime FechaSubida { get; set; } = DateTime.Now;

    // Relaciones
    [ForeignKey("IdViaje")]
    public Viaje Viaje { get; set; }
}
