using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Text;

namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess
{
    /// <summary>
    /// Clase que proporciona métodos para ejecutar procedimientos almacenados en la base de datos.
    /// Facilita la ejecución de operaciones CRUD y consultas a través de stored procedures.
    /// </summary>
    public class EjecutorDeProcedimientosAlmacenados
    {
        private readonly ComunicacionesDbContext _elContextoDeBaseDeDatos;

        /// <summary>
        /// Constructor que inicializa el ejecutor con el contexto de base de datos.
        /// </summary>
        /// <param name="contextoDeBaseDeDatos">Contexto de Entity Framework para la base de datos</param>
        public EjecutorDeProcedimientosAlmacenados(ComunicacionesDbContext contextoDeBaseDeDatos)
        {
            _elContextoDeBaseDeDatos = contextoDeBaseDeDatos;
        }

        /// <summary>
        /// Construye un EXEC por nombre, evitando el binding por posición:
        /// EXEC sp @p = @p, @q = @q, ...
        /// Esto evita errores como: nvarchar -> bit cuando el orden de parámetros varía.
        /// </summary>
        private static string ConstruirEjecucionNombrada(string nombreDeProcedimientoAlmacenado, SqlParameter[] losParametros)
        {
            if (string.IsNullOrWhiteSpace(nombreDeProcedimientoAlmacenado))
            {
                throw new ArgumentException("nombreDeProcedimientoAlmacenado no puede ser vacío.", nameof(nombreDeProcedimientoAlmacenado));
            }

            if (losParametros == null || losParametros.Length == 0)
            {
                return $"EXEC {nombreDeProcedimientoAlmacenado}";
            }

            var elConstructorDeTexto = new StringBuilder();
            elConstructorDeTexto.Append("EXEC ").Append(nombreDeProcedimientoAlmacenado).Append(' ');

            for (int indiceDeParametro = 0; indiceDeParametro < losParametros.Length; indiceDeParametro++)
            {
                var elParametro = losParametros[indiceDeParametro] ?? throw new ArgumentNullException(nameof(losParametros), "Un parámetro es null.");

                if (indiceDeParametro > 0)
                {
                    elConstructorDeTexto.Append(", ");
                }

                // @param = @param
                elConstructorDeTexto.Append(elParametro.ParameterName)
                  .Append(" = ")
                  .Append(elParametro.ParameterName);

                // Manejar parámetros OUTPUT
                if ((elParametro.Direction & ParameterDirection.Output) != 0)
                {
                    elConstructorDeTexto.Append(" OUTPUT");
                }
            }

            return elConstructorDeTexto.ToString();
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve una colección de entidades.
        /// Útil para operaciones SELECT que retornan múltiples registros.
        /// </summary>
        public async Task<List<T>> EjecutarProcedimientoAlmacenadoAsync<T>(string nombreDeProcedimientoAlmacenado, params SqlParameter[] losParametros) where T : class
        {
            var laConsultaSql = ConstruirEjecucionNombrada(nombreDeProcedimientoAlmacenado, losParametros);
            return await _elContextoDeBaseDeDatos.Set<T>().FromSqlRaw(laConsultaSql, losParametros).ToListAsync();
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que no retorna resultados (INSERT/UPDATE/DELETE).
        /// </summary>
        public async Task EjecutarProcedimientoAlmacenadoSinResultadoAsync(string nombreDeProcedimientoAlmacenado, params SqlParameter[] losParametros)
        {
            var laConsultaSql = ConstruirEjecucionNombrada(nombreDeProcedimientoAlmacenado, losParametros);
            await _elContextoDeBaseDeDatos.Database.ExecuteSqlRawAsync(laConsultaSql, losParametros);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que no devuelve resultados.
        /// Ideal para INSERT/UPDATE/DELETE. Soporta OUTPUT.
        /// Devuelve el número de filas afectadas.
        /// </summary>
        public async Task<int> EjecutarProcedimientoAlmacenadoConFilasAfectadasAsync(string nombreDeProcedimientoAlmacenado, params SqlParameter[] losParametros)
        {
            var laConsultaSql = ConstruirEjecucionNombrada(nombreDeProcedimientoAlmacenado, losParametros);
            return await _elContextoDeBaseDeDatos.Database.ExecuteSqlRawAsync(laConsultaSql, losParametros);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve una única entidad.
        /// Útil para SELECT single (o SPs que retornan una sola fila).
        /// </summary>
        public async Task<T?> EjecutarProcedimientoAlmacenadoUnicoAsync<T>(string nombreDeProcedimientoAlmacenado, params SqlParameter[] losParametros) where T : class
        {
            var laConsultaSql = ConstruirEjecucionNombrada(nombreDeProcedimientoAlmacenado, losParametros);
            var losResultados = await _elContextoDeBaseDeDatos.Database.SqlQueryRaw<T>(laConsultaSql, losParametros).ToListAsync();
            return losResultados.FirstOrDefault();
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado y devuelve una lista de DTOs usando SqlQueryRaw.
        /// (DTOs no necesariamente mapeados en DbContext)
        /// </summary>
        public async Task<List<T>> EjecutarProcedimientoAlmacenadoComoDtoAsync<T>(string nombreDeProcedimientoAlmacenado, params SqlParameter[] losParametros) where T : class
        {
            var laConsultaSql = ConstruirEjecucionNombrada(nombreDeProcedimientoAlmacenado, losParametros);
            return await _elContextoDeBaseDeDatos.Database.SqlQueryRaw<T>(laConsultaSql, losParametros).ToListAsync();
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve una lista de objetos NO mapeados en el DbContext.
        /// Ideal para DTOs personalizados.
        /// </summary>
        public async Task<List<T>> EjecutarProcedimientoAlmacenadoComoDtoNoMapeadoAsync<T>(string nombreDeProcedimientoAlmacenado, params SqlParameter[] losParametros) where T : class
        {
            var laConsultaSql = ConstruirEjecucionNombrada(nombreDeProcedimientoAlmacenado, losParametros);
            return await _elContextoDeBaseDeDatos.Database.SqlQueryRaw<T>(laConsultaSql, losParametros).ToListAsync();
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado con DbDataReader (multi-result o mapeo manual).
        /// Este método ya usa CommandType.StoredProcedure (forma recomendada).
        /// </summary>
        public async Task<TResult> EjecutarProcedimientoAlmacenadoConMultiplesResultadosAsync<TResult>(
            string nombreDeProcedimientoAlmacenado,
            Func<DbDataReader, TResult> funcionDeMapeo,
            params SqlParameter[] losParametros)
        {
            if (string.IsNullOrWhiteSpace(nombreDeProcedimientoAlmacenado))
            {
                throw new ArgumentException("nombreDeProcedimientoAlmacenado no puede ser vacío.", nameof(nombreDeProcedimientoAlmacenado));
            }

            if (funcionDeMapeo == null)
            {
                throw new ArgumentNullException(nameof(funcionDeMapeo));
            }

            var laConexion = _elContextoDeBaseDeDatos.Database.GetDbConnection();

            try
            {
                if (laConexion.State != ConnectionState.Open)
                {
                    await laConexion.OpenAsync();
                }

                using var elComando = laConexion.CreateCommand();
                elComando.CommandText = nombreDeProcedimientoAlmacenado;
                elComando.CommandType = CommandType.StoredProcedure;

                if (losParametros != null && losParametros.Length > 0)
                {
                    elComando.Parameters.AddRange(losParametros);
                }

                using var elLector = await elComando.ExecuteReaderAsync();
                var elResultado = funcionDeMapeo(elLector);
                return elResultado;
            }
            finally
            {
                if (laConexion.State == ConnectionState.Open)
                {
                    await laConexion.CloseAsync();
                }
            }
        }
    }
}
