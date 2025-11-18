using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess
{
    public class StoredProcedureExecutor
    {
        private readonly ComunicacionesDbContext _context;

        public StoredProcedureExecutor(ComunicacionesDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve una lista de objetos.
        /// </summary>
        public async Task<List<T>> ExecuteStoredProcedureAsync<T>(string spName, params object[] parameters) where T : class
        {
            string sqlQuery = BuildSqlQuery(spName, parameters.Length);
            return await _context.Set<T>().FromSqlRaw(sqlQuery, parameters).ToListAsync();
        }


        public async Task<List<T>> ExecuteStoredProcedureAsync<T>(
        string spName,
        params SqlParameter[] parameters) where T : class
        {
            // Genera: EXEC comunicaciones.Consultar_Conversaciones_Usuario @id_usuario, @skip, @take
            var paramNames = parameters != null && parameters.Length > 0
                ? string.Join(", ", parameters.Select(p => p.ParameterName))
                : string.Empty;

            var sqlQuery = string.IsNullOrWhiteSpace(paramNames)
                ? $"EXEC {spName}"
                : $"EXEC {spName} {paramNames}";

            return await _context.Set<T>()
                .FromSqlRaw(sqlQuery, parameters)
                .ToListAsync();
        }
        /// <summary>
        /// Ejecuta un procedimiento almacenado que no devuelve resultados (INSERT, UPDATE, DELETE).
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string spName, params SqlParameter[] parameters)
        {
            using var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString);
            using var command = new SqlCommand(spName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Construye la consulta SQL con parámetros dinámicos.
        /// </summary>
        private string BuildSqlQuery(string spName, int paramCount)
        {
            var paramPlaceholders = string.Join(", ", Enumerable.Range(0, paramCount).Select(i => $"@p{i}"));
            return paramCount > 0 ? $"EXEC {spName} {paramPlaceholders}" : $"EXEC {spName}";
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve un solo objeto.
        /// </summary>
        public async Task<T?> ExecuteStoredProcedureSingleAsync<T>(string spName, params object[] parameters) where T : class
        {
            string sqlQuery = BuildSqlQuery(spName, parameters.Length);
            var result = await _context.Database.SqlQueryRaw<T>(sqlQuery, parameters).ToListAsync();
            return result.FirstOrDefault();
        }

        public async Task<T?> ExecuteStoredProcedureSingleAsync<T>(string spName, Dictionary<string, object> parameters) where T : class
        {
            var sqlParams = parameters
                .Select(kv => new SqlParameter(kv.Key, kv.Value ?? DBNull.Value))
                .ToArray();

            string sqlQuery = $"EXEC {spName} " + string.Join(", ", sqlParams.Select(p => p.ParameterName));

            var result = await _context.Database.SqlQueryRaw<T>(sqlQuery, sqlParams).ToListAsync();
            return result.FirstOrDefault();
        }


        public async Task<TResult> ExecuteStoredProcedureMultipleAsync<TResult>(string storedProcedureName, Func<DbDataReader, TResult> mapFunction, params SqlParameter[] parameters)
        {
            var connection = _context.Database.GetDbConnection();

            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = storedProcedureName;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);

                using var reader = await command.ExecuteReaderAsync();
                var result = mapFunction(reader);
                return result;
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    await connection.CloseAsync();
            }
        }



        public async Task<int> ExecuteNonQueryAsync(string spName, params object[] parameters)
        {
            string sqlQuery = BuildSqlQuery(spName, parameters.Length);
            return await _context.Database.ExecuteSqlRawAsync(sqlQuery, parameters);
        }


    }
}
