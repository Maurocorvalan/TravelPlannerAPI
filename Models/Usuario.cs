using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuario
{
    [Key]
    [Column("id_usuario")]

    public int IdUsuario { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    [EmailAddress]
    public string Correo { get; set; }

    [Required]
    public string Contrasena { get; set; }

    [Column("fecha_registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Relaciones
    public List<Viaje>? Viajes { get; set; }
    public List<Notificacion>? Notificaciones { get; set; }
    public List<Preferencia>? Preferencias { get; set; }
}
