using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RegistroEstudiantes.Models;
using RegistroEstudiantes.Models.ViewModels;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace RegistroEstudiantes.Services
{
    public class RegistrosServices : IRegistrosServices
    {
        private readonly EstudiantesContext _context;
        private readonly IMapper _mapper;
        public RegistrosServices(EstudiantesContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public Respuesta Crear_estudiante(Estudiante estudiante)
        {
            if (_context.Estudiante.Any(e => estudiante.NumIdentificacion == e.NumIdentificacion))
                return new Respuesta { mensaje = $"El estudiante con identificacion {estudiante.NumIdentificacion} ya existe",aprobacion=false,tipo_mensaje = "MensajeError" };
            _context.Estudiante.Add(estudiante);
            _context.SaveChanges();
            return new Respuesta { mensaje = $"Estudiante creado con éxito",aprobacion=true,tipo_mensaje = "MensajeConfirmacion" };

        }

        public IList<ViewModel_Estudiantes_Profesor> ObtenerEstudiantes(int id_materia)
        {
            List<ViewModel_Estudiantes_Profesor> estudiantes = _context.Registro
            .Where(r => r.IdMateria == id_materia)
             .Join(_context.Profesor,
                r => r.IdMateriaNavigation.IdProfesor,
                p => p.IdProfesor,
                (r, p) => new ViewModel_Estudiantes_Profesor
                {
                    IdEstudiante = r.IdEstudiante,
                    NombreEstudiante = $"{r.IdEstudianteNavigation.Nombre} {r.IdEstudianteNavigation.Apellido}",                 
                    Materia = r.IdMateriaNavigation.NombreMateria,
                    NombreProfesor = $"{p.Nombre} {p.Apellido}"
                 
                }).ToList();           
        

            return estudiantes;
        }

        public IList<Registro> ObtenerRegistros(int id_estudiante)
        {
            List<Registro> registros_estudiante = _context.Registro.Where(e => e.IdEstudiante == id_estudiante).ToList();
            return registros_estudiante;
        }


        public IList<Materia> ObtenerMaterias(int id_estudiante)
        {

            List<Materia> materias = _context.Registro
            .Where(r => r.IdEstudiante == id_estudiante)
            .Select(r => new Materia
            {
                IdMateria = r.IdMateria,
                NombreMateria = r.IdMateriaNavigation.NombreMateria,
                NumCreditos = r.IdMateriaNavigation.NumCreditos,
                IdProfesor = r.IdMateriaNavigation.IdProfesor
            })
            .ToList();

            return materias;
        }

        public ViewModel_MateriasEstudiante ObtenerMaterias()
        {
            List<Materia> materias = _context.Materia.ToList();
            ViewModel_MateriasEstudiante viewModel_MateriasEstudiante = new ViewModel_MateriasEstudiante();
            List<SelectListItem> lista_materias = materias.OrderBy(n => n.NombreMateria)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.IdMateria.ToString(),
                    Text = n.NombreMateria,
                }).ToList();
            viewModel_MateriasEstudiante.Materias = lista_materias;
            return viewModel_MateriasEstudiante;
        }

        public ViewModel_MateriasEstudiante Obtener_MateriasProfesor()
        {
            var materias = _context.Materia
                .Join(
                    _context.Profesor,
                    m => m.IdProfesor,
                    p => p.IdProfesor,
                    (m, p) => new ViewModel_MateriasProfesores
                    {
                        IdMateria = m.IdMateria,
                        NombreMateria = m.NombreMateria,              
                        NombreProfesor = p.Nombre,  
                        ApellidoProfesor = p.Apellido
                    })
                .ToList();
            ViewModel_MateriasEstudiante viewModel_MateriasEstudiante = new ViewModel_MateriasEstudiante();
            List<SelectListItem> lista_materias = materias.OrderBy(n => n.NombreMateria)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.IdMateria.ToString(),
                    Text = $"{n.NombreMateria} ({n.NombreProfesor} {n.ApellidoProfesor})"
                }).ToList();
            viewModel_MateriasEstudiante.Materias = lista_materias;
            return viewModel_MateriasEstudiante;
        }

        public ViewModel_MateriasEstudiante ObtenerMaterias_Estudiante(int id_estudiante)
        {
            var materias = ObtenerMaterias(id_estudiante);
            ViewModel_MateriasEstudiante viewModel_MateriasEstudiante = new ViewModel_MateriasEstudiante();

            List<SelectListItem> lista_materias = materias.OrderBy(n => n.NombreMateria)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.IdMateria.ToString(),
                    Text = n.NombreMateria,
                }).ToList();
            viewModel_MateriasEstudiante.Materias = lista_materias;
            return viewModel_MateriasEstudiante;
        }

      
        public Respuesta RegistrarMaterias(string identificacion, List<int> id_materias)
        {
            int id_estudiante, cupos_materias;
            Respuesta respuesta=new Respuesta();
            Estudiante estudiante = _context.Estudiante.FirstOrDefault(e => e.NumIdentificacion == identificacion);
            id_estudiante = estudiante == null ? 0 : estudiante.IdEstudiante;
            IList<Materia> materias_inscritas = ObtenerMaterias(id_estudiante);
            cupos_materias = Materias_restantes(materias_inscritas.Count);

            if (cupos_materias.Equals(0))
                return (new Respuesta { aprobacion = false, mensaje = "No tiene cupo para inscribir mas materias",tipo_mensaje = "MensajeError" });

            respuesta = Validar_Materias(id_materias);
            if(!respuesta.aprobacion)
                return (respuesta);

            respuesta = Validar_Profesor(id_estudiante, id_materias);
            if(!respuesta.aprobacion)
                return (respuesta);

            List<Registro> lista_registros = new List<Registro>();
            Registro registro;
            foreach (int id in id_materias)
            {
                registro = new Registro();
                registro.IdMateria = id;
                registro.IdEstudiante = id_estudiante;
                lista_registros.Add(registro);
            }

            _context.Registro.AddRange(lista_registros);
            _context.SaveChanges();
            return (new Respuesta { aprobacion = true, mensaje = "Materias registradas correctamente",tipo_mensaje= "MensajeConfirmacion" });

        }

        protected int Materias_restantes(int num_registros)
        {
            int materias_restantes = 3;
            materias_restantes -= num_registros;
            return materias_restantes;
        }

        protected Respuesta Materias_Inscritas(IList<Materia> materias, List<int> id_materias)
        {            
            if (materias.Count > 0)
            {
                List<Materia> materiasCoincidentes = materias.Where(m => id_materias.Contains(m.IdMateria)).ToList();
                if (materiasCoincidentes.Count > 0)
                {
                    switch (materiasCoincidentes.Count)
                    {
                        case 1: return new Respuesta { mensaje = $"La materia '{materiasCoincidentes[0].NombreMateria}' ya se encuentra inscrita", aprobacion = false , tipo_mensaje = "MensajeError" };
                        case 2: return new Respuesta { mensaje = $"Las materias '{materiasCoincidentes[1].NombreMateria}' y '{materiasCoincidentes[0].NombreMateria}' ya se encuentran inscritas", aprobacion = false , tipo_mensaje = "MensajeError" };
                    }                       

                }
                

            }
            return new Respuesta { mensaje = string.Empty, aprobacion = true, tipo_mensaje = "MensajeConfirmacion" };
        }

        protected Respuesta Validar_Profesor(int id_estudiante,List<int> id_materias_por_inscribir)
        {            

            var id_profesores_materias = _context.Materia.Where(id => id_materias_por_inscribir.Contains(id.IdMateria))
                .Select(id=>id.IdProfesor)
                .ToList();

            var profesoresRepetidos = id_profesores_materias
                           .GroupBy(id => id)
                           .Where(group => group.Count() > 1)
                           .Select(group => group.Key)
                           .ToList();

            if (profesoresRepetidos.Count > 0)
                return new Respuesta { mensaje = $"No puede inscribir dos o mas materias con el mismo profesor", aprobacion = false,tipo_mensaje= "MensajeError" };               

            return new Respuesta { mensaje = string.Empty, aprobacion = true,tipo_mensaje= "MensajeConfirmacion" };

        }

        protected Respuesta Validar_Materias(List<int> id_materias_por_inscribir)
        {

            var materiasRepetidas = id_materias_por_inscribir
                         .GroupBy(id => id)
                         .Where(group => group.Count() > 1)
                         .Select(group => group.Key)
                         .ToList();
            if(materiasRepetidas.Count>0)
                return new Respuesta { mensaje = $"No puede inscribir la misma materia dos veces", aprobacion = false, tipo_mensaje="MensajeError" };

            return new Respuesta { mensaje = string.Empty, aprobacion = true ,tipo_mensaje= "MensajeConfirmacion" };

        }


    }
}
