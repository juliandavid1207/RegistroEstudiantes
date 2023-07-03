using RegistroEstudiantes.Models;
using RegistroEstudiantes.Models.ViewModels;

namespace RegistroEstudiantes.Services
{
    public interface IRegistrosServices
    {
        public IList<ViewModel_Estudiantes_Profesor> ObtenerEstudiantes(int id_materia);     

        public ViewModel_MateriasEstudiante ObtenerMaterias_Estudiante(int id_estudiante);

        public Respuesta Crear_estudiante(Estudiante estudiante);

        public ViewModel_MateriasEstudiante ObtenerMaterias();

        public Respuesta RegistrarMaterias(string identificacion,List<int> id_materias);

        public ViewModel_MateriasEstudiante Obtener_MateriasProfesor();

    }
}
