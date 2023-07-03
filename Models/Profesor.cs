using System;
using System.Collections.Generic;

namespace RegistroEstudiantes.Models;

public partial class Profesor
{
    public int IdProfesor { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public virtual Materia IdProfesorNavigation { get; set; } = null!;
}
