using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegistroEstudiantes.Models;
using RegistroEstudiantes.Models.ViewModels;
using RegistroEstudiantes.Services;

namespace RegistroEstudiantes.Controllers
{
    public class MateriasController : Controller
    {
        private readonly IRegistrosServices _services;
        private readonly EstudiantesContext _context;
        public MateriasController(IRegistrosServices services, EstudiantesContext context)
        {
            _services = services;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewModel_MateriasEstudiante _Materias = await Task.Run(() => _services.Obtener_MateriasProfesor());
            return View(_Materias);
        }

        [HttpPost]
        public IActionResult Create([FromForm] string NumIdentificacion, string materia1, string materia2,string materia3)
        {
            if (NumIdentificacion != null)
            {
                try
                {
                    bool existe = _context.Estudiante.Any(e => e.NumIdentificacion == NumIdentificacion);
                    if (existe)
                    {
                        Respuesta respuesta = _services.RegistrarMaterias(NumIdentificacion, new List<int> { Convert.ToInt32(materia1.Trim()), Convert.ToInt32(materia2.Trim()), Convert.ToInt32(materia3.Trim()) });
                        TempData[respuesta.tipo_mensaje] = respuesta.mensaje.ToString();
                        return RedirectToAction("Index");
                    }
                    TempData["MensajeError"] = $"El estudiante con numero de identificación {NumIdentificacion} no esta creado";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {                
                    TempData["MensajeError"] = $"Error al crear el registro: {ex.Message}";
                    return RedirectToAction("Index");
                }
            }

            TempData["MensajeError"] = "Documento no valido";
            return RedirectToAction("Index");

        }
    }
}
