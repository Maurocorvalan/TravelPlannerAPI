using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Notificacion
{
    [Key]
    public int IdNotificacion { get; set; }

    [Required]
    public int IdUsuario { get; set; }

    [Required]
    public string Mensaje { get; set; }

    public DateTime FechaEnvio { get; set; } = DateTime.Now;

    public bool Leido { get; set; } = false;

    // Relaciones
    [ForeignKey("IdUsuario")]
    public Usuario Usuario { get; set; }
}
