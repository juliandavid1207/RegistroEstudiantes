using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using RegistroEstudiantes.Models;
using RegistroEstudiantes.Models.ViewModels;
using RegistroEstudiantes.Services;

namespace RegistroEstudiantes.Controllers
{
    public class RegistrosController : Controller
    {
        private readonly IRegistrosServices _registrosServices;
        private readonly EstudiantesContext _context;
        

        public RegistrosController(IRegistrosServices registrosServices, EstudiantesContext context)
        {
            _registrosServices = registrosServices;
            _context = context;
        
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> ListaEstudiantes([FromBody]int selectedValue)
        {            
            var estudiantes = await Task.Run(() => _registrosServices.ObtenerEstudiantes(selectedValue));
            return PartialView("ListaEstudiantes", estudiantes);
        }

        public async Task<IActionResult> ListaMaterias([FromBody]int idEstudiante)
        {
            int id_estudiante = _context.Estudiante.Where(e => e.NumIdentificacion == idEstudiante.ToString())
                .Select(e=>e.IdEstudiante)
                .FirstOrDefault();

           
            ViewModel_MateriasEstudiante _MateriasEstudiante = await Task.Run(() => _registrosServices.ObtenerMaterias_Estudiante(id_estudiante));
            return PartialView("ListaMaterias", _MateriasEstudiante);
            
        }






    }
}
