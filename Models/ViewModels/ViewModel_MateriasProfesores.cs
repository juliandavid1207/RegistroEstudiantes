namespace RegistroEstudiantes.Models.ViewModels
{
    public class ViewModel_MateriasProfesores
    {
        public int IdMateria { get; set; }

        public string NombreMateria { get; set; } = null!;
        public string? NombreProfesor { get; set; }

        public string? ApellidoProfesor { get; set; }
    }
}
