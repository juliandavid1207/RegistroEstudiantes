using System;
using System.Collections.Generic;

namespace RegistroEstudiantes.Models;

public partial class Materia
{
    public int IdMateria { get; set; }

    public string NombreMateria { get; set; } = null!;

    public int NumCreditos { get; set; }

    public int IdProfesor { get; set; }

    public virtual Profesor? Profesor { get; set; }

    public virtual ICollection<Registro> Registros { get; set; } = new List<Registro>();
}
