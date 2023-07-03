using System;
using System.Collections.Generic;

namespace RegistroEstudiantes.Models;

public partial class Registro
{
    public int IdRegistro { get; set; }

    public int IdEstudiante { get; set; }

    public int IdMateria { get; set; }

    public virtual Estudiante IdEstudianteNavigation { get; set; } = null!;

    public virtual Materia IdMateriaNavigation { get; set; } = null!;


}
