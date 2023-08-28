using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models
{
    public class CuentaCreacionDTO: Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
