using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task Crear(Transaccion transaccion);
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId);
        Task Borrar(int id);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta model);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT tr.*, cat.TipoOperacionId
                  FROM Transacciones tr
                  INNER JOIN Categorias cat
                  ON tr.CategoriaId = cat.Id
                  WHERE tr.Id = @Id AND tr.UsuarioId = @UsuarioId;", new {id, usuarioId});
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"TransaccionesInsertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.Monto,
                    transaccion.FechaTransaccion,
                    transaccion.Nota,
                    transaccion.CuentaId,
                    transaccion.CategoriaId
                }, commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, t.Monto, t.FechaTransaccion, t.CategoriaId, t.Nota, c.TipoOperacionId, c.Nombre as Categoria, ct.Nombre as Cuenta
                                                              FROM Transacciones t
                                                              INNER JOIN Categorias c
                                                              ON t.CategoriaId = c.Id
                                                              INNER JOIN Cuentas ct
                                                              ON ct.Id = t.CuentaId
                                                              WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId
                                                              AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("TransaccionesActualizar", new
            {
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("TransaccionesBorrar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
