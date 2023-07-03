using Microsoft.AspNetCore.Mvc;
using RegistroEstudiantes.Models;
using RegistroEstudiantes.Services;

namespace RegistroEstudiantes.Controllers
{
    public class EstudianteController : Controller
    { 
        private readonly IRegistrosServices _registrosServices;
        public EstudianteController(IRegistrosServices registrosServices) {          
            _registrosServices = registrosServices;
        }
        public ActionResult Index(Estudiante estudiante)
        {
             return View(estudiante);
        }

        [HttpPost]
    
        public IActionResult Create([FromForm] Estudiante estudiante)
        {
            Respuesta respuesta = new Respuesta { mensaje = "Datos no validos", aprobacion = false };
            if (ModelState.IsValid)
            {
                respuesta= _registrosServices.Crear_estudiante(estudiante);
                TempData[respuesta.tipo_mensaje] = respuesta.mensaje;
                if (respuesta.aprobacion)
                {                  
                    return RedirectToAction("Index",new Estudiante { });
                }               
            }
           
            return RedirectToAction("Index", estudiante);

        }
    }
}
