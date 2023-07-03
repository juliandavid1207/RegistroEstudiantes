using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RegistroEstudiantes.Models;

public partial class Estudiante
{
    public int IdEstudiante { get; set; }

    [Required]
    public string? Nombre { get; set; }

    [Required]
    public string? Apellido { get; set; }

    [Required]
    public string? NumIdentificacion { get; set; }

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
}
