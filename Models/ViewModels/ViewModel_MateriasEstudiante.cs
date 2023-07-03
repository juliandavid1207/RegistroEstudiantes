using Microsoft.AspNetCore.Mvc.Rendering;

namespace RegistroEstudiantes.Models.ViewModels
{
    public class ViewModel_MateriasEstudiante
    { 
        public IEnumerable<SelectListItem> Materias { get; set; }
    }
}
