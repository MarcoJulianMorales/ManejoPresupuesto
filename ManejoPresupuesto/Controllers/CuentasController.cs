using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;
        private readonly IServicioReportes servicioReportes;

        public CuentasController(
            IRepositorioTiposCuentas repositorioTiposCuentas,
            IServicioUsuarios servicioUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioTransacciones repositorioTransacciones,
            IMapper mapper,
            IServicioReportes servicioReportes)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.servicioReportes = servicioReportes;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuenta = await repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasDTO()
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);
        }

        public async Task<IActionResult> Detalle(int id, int mes, int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)

                return RedirectToAction("NoEncontrado", "Home");
            
            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladasPorCuenta(usuarioId, id, mes, año, ViewBag);

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new CuentaCreacionDTO();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionDTO cuentaCreacionDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = repositorioTiposCuentas.Obtener(usuarioId);

            if (tipoCuenta == null)
                return RedirectToAction("NoEncontrado", "Home");

            if (!ModelState.IsValid)
            {
                cuentaCreacionDTO.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuentaCreacionDTO);
            }

            await repositorioCuentas.Crear(cuentaCreacionDTO);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta == null)
                return RedirectToAction("NoEncontrado", "Home");

            var modelo = mapper.Map<CuentaCreacionDTO>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionDTO cuentaCreacionDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaCreacionDTO.Id, usuarioId);

            if (cuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaCreacionDTO.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            await repositorioCuentas.Actualizar(cuentaCreacionDTO);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            await repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}