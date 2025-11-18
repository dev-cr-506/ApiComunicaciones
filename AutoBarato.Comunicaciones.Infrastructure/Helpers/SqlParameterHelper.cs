using  Microsoft.Data.SqlClient;
using  System.ComponentModel.DataAnnotations.Schema;
using  System.Data;
using  System.Reflection;
using  System.Text;

namespace AutoBarato.Comunicaciones.Infrastructure.Helpers
{
    /// <summary>
    /// Clase auxiliar que proporciona métodos para crear y manipular parámetros SQL.
    /// Facilita la creación de parámetros estructurados y la conversión de objetos a parámetros SQL.
    /// </summary>
    public static class SqlParameterHelper
    {
        /// <summary>
        /// Crea un parámetro SQL estructurado (Table-Valued Parameter) a partir de una colección de objetos.
        /// Útil para enviar múltiples registros a un procedimiento almacenado en una sola llamada.
        /// </summary>
        /// <typeparam name="T">Tipo de objeto que representa cada fila de datos</typeparam>
        /// <param name="parameterName">Nombre del parámetro en el procedimiento almacenado</param>
        /// <param name="typeName">Nombre del tipo de tabla definido en SQL Server</param>
        /// <param name="data">Colección de objetos que se convertirán en filas</param>
        /// <returns>SqlParameter configurado como parámetro de tabla</returns>
        /// <example>
        /// var equipamientos = new List<EquipamientoAuto> { ... };
        /// var tvp = SqlParameterHelper.CreateStructured("@Equipamientos", "dbo.EquipamientoTableType", equipamientos);
        /// </example>
        public static SqlParameter CreateStructured<T>(string parameterName, string typeName, IEnumerable<T> data)
        {
            var dataTable = new DataTable();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataTable.Columns.Add(prop.Name, type);
            }

            foreach (var item in data)
            {
                var values = props.Select(p => p.GetValue(item, null)).ToArray();
                dataTable.Rows.Add(values);
            }

            return new SqlParameter(parameterName, SqlDbType.Structured)
            {
                TypeName = typeName,
                Value = dataTable
            };
        }

        /// <summary>
        /// Crea un parámetro SQL de tipo entero.
        /// Método de conveniencia para crear parámetros enteros de forma rápida.
        /// </summary>
        /// <param name="name">Nombre del parámetro</param>
        /// <param name="value">Valor entero</param>
        /// <returns>SqlParameter configurado como entero</returns>
        /// <example>
        /// var idParam = SqlParameterHelper.CreateInt("@IdAuto", 123);
        /// </example>
        public static SqlParameter CreateInt(string name, int value)
            => new SqlParameter(name, SqlDbType.Int) { Value = value };

        /// <summary>
        /// Convierte un objeto en un array de parámetros SQL.
        /// Útil para mapear automáticamente propiedades de objetos a parámetros de procedimientos almacenados.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a convertir</typeparam>
        /// <param name="obj">Objeto que contiene los valores para los parámetros</param>
        /// <param name="mapping">Diccionario opcional para mapear nombres de propiedades a nombres de parámetros SQL</param>
        /// <param name="ignoreNulls">Si es true, ignora las propiedades con valor null</param>
        /// <param name="excludeProps">Array opcional de nombres de propiedades a excluir</param>
        /// <returns>Array de SqlParameter creados a partir del objeto</returns>
        /// <example>
        /// var auto = new Auto { Marca = "Toyota", Modelo = "Corolla" };
        /// var parameters = SqlParameterHelper.FromObject(auto, 
        ///     mapping: new Dictionary<string, string> { {"Marca", "marca_auto"} },
        ///     ignoreNulls: true);
        /// </example>
        public static SqlParameter[] FromObject<T>(T obj, Dictionary<string, string>? mapping = null, bool ignoreNulls = false, string[]? excludeProps = null)
        {
            var props = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null) // Ignora propiedades marcadas como [NotMapped]
                .ToList();

            var parameters = new List<SqlParameter>();

            foreach (var prop in props)
            {
                if (excludeProps?.Contains(prop.Name) == true)
                    continue;

                var value = prop.GetValue(obj);

                if (ignoreNulls && value == null)
                    continue;

                var paramName = mapping != null && mapping.TryGetValue(prop.Name, out var customName)
                    ? $"@{customName}"
                    : $"@{ToSnakeCase(prop.Name)}"; // Convierte a snake_case si no hay mapping

                parameters.Add(new SqlParameter(paramName, value ?? DBNull.Value));
            }

            return parameters.ToArray();
        }

        /// <summary>
        /// Convierte una cadena en formato PascalCase a snake_case.
        /// Utilizado internamente para convertir nombres de propiedades C# a nombres de parámetros SQL.
        /// </summary>
        /// <param name="input">Cadena en formato PascalCase</param>
        /// <returns>Cadena convertida a snake_case</returns>
        /// <example>
        /// "NombreUsuario" -> "nombre_usuario"
        /// "IDCliente" -> "id_cliente"
        /// </example>
        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(input[0]));

            for (int i = 1; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsUpper(c))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}