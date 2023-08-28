using Dapper;
using ManejoPresupuesto.Models.TiposCuentas;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Crear(TipoCuenta tipoCuenta);
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados);
        Task<bool> Existe(string nombre, int usuarioId);
    }
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration) 
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                             FROM TiposCuentas
                                                             WHERE UsuarioId = @UsuarioId
                                                             ORDER BY Orden", new { usuarioId });
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                               FROM TiposCuentas
                                                               WHERE Id = @Id AND UsuarioId = @UsuarioId",
                                                               new { id, usuarioId });

        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar", 
                new {usuarioId = tipoCuenta.UsuarioId, nombre = tipoCuenta.Nombre},
                commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id = @Id", tipoCuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM TiposCuentas WHERE Id = @Id;", new {id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tiposCuentasOrdenados)
        {
            var query = @"UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tiposCuentasOrdenados);

        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                           FROM TiposCuentas
                                                                           WHERE Nombre = @Nombre and UsuarioId = @UsuarioId;",
                                                                           new { nombre, usuarioId });
            return existe == 1 ? true : false;
        }
    }
}
