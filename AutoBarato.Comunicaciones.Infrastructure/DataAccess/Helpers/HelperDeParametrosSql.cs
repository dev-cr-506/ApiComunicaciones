using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text;

namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess.Helpers
{
    /// <summary>
    /// Clase auxiliar que proporciona métodos para crear y manipular parámetros SQL.
    /// Facilita la creación de parámetros estructurados y la conversión de objetos a parámetros SQL.
    /// </summary>
    public static class HelperDeParametrosSql
    {
        public static SqlParameter CrearEstructurado<T>(string nombreDeParametro, string nombreDeTipo, IEnumerable<T> datos)
        {
            var laTablaDeDatos = new DataTable();
            var lasPropiedades = typeof(T).GetProperties();

            foreach (var laPropiedad in lasPropiedades)
            {
                var elTipo = Nullable.GetUnderlyingType(laPropiedad.PropertyType) ?? laPropiedad.PropertyType;
                laTablaDeDatos.Columns.Add(laPropiedad.Name, elTipo);
            }

            foreach (var elElemento in datos)
            {
                var losValores = lasPropiedades.Select(p => p.GetValue(elElemento, null)).ToArray();
                laTablaDeDatos.Rows.Add(losValores);
            }

            return new SqlParameter(nombreDeParametro, SqlDbType.Structured)
            {
                TypeName = nombreDeTipo,
                Value = laTablaDeDatos
            };
        }

        public static SqlParameter CrearEntero(string nombre, int valor)
            => new SqlParameter(nombre, SqlDbType.Int) { Value = valor };

        public static SqlParameter CrearFecha(string nombre, DateTime valor)
            => new SqlParameter(nombre, SqlDbType.Date)
            {
                Value = valor.Date
            };

        public static SqlParameter CrearEnteroNulo(string nombre, int? valor)
            => new SqlParameter(nombre, SqlDbType.Int) { Value = valor ?? (object)DBNull.Value };

        public static SqlParameter CrearVarchar(string nombre, string valor, int largo = 50)
            => new SqlParameter(nombre, SqlDbType.VarChar, largo) { Value = valor ?? (object)DBNull.Value };

        public static SqlParameter CrearGuid(string nombre, Guid valor)
            => new SqlParameter(nombre, SqlDbType.UniqueIdentifier) { Value = valor };

        public static SqlParameter CrearGuidNulo(string nombre, Guid? valor)
            => new SqlParameter(nombre, SqlDbType.UniqueIdentifier) { Value = valor ?? (object)DBNull.Value };

        public static SqlParameter CrearNvarchar(string nombre, string? valor, int largo = -1)
            => new SqlParameter(nombre, SqlDbType.NVarChar, largo) { Value = (object?)valor ?? DBNull.Value };

        public static SqlParameter CrearFechaHora(string nombre, DateTime valor)
            => new SqlParameter(nombre, SqlDbType.DateTime2) { Value = valor };

        public static SqlParameter CrearFechaHoraNula(string nombre, DateTime? valor)
            => new SqlParameter(nombre, SqlDbType.DateTime2) { Value = (object?)valor ?? DBNull.Value };

        public static SqlParameter[] CrearDesdeObjeto<T>(
            T objeto,
            Dictionary<string, string>? mapeo = null,
            bool ignorarNulos = false,
            string[]? propiedadesExcluidas = null)
        {
            var lasPropiedades = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
                .ToList();

            var losParametros = new List<SqlParameter>();

            foreach (var laPropiedad in lasPropiedades)
            {
                if (propiedadesExcluidas?.Contains(laPropiedad.Name) == true)
                {
                    continue;
                }

                var elValor = laPropiedad.GetValue(objeto);

                if (ignorarNulos && elValor == null)
                {
                    continue;
                }

                var elNombreDeParametro = mapeo != null && mapeo.TryGetValue(laPropiedad.Name, out var customName)
                    ? $"@{customName}"
                    : $"@{ConvertirASnakeCase(laPropiedad.Name)}";

                losParametros.Add(new SqlParameter(elNombreDeParametro, elValor ?? DBNull.Value));
            }

            return losParametros.ToArray();
        }

        private static string ConvertirASnakeCase(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return texto;
            }

            var elConstructorDeTexto = new StringBuilder();
            elConstructorDeTexto.Append(char.ToLowerInvariant(texto[0]));

            for (int i = 1; i < texto.Length; i++)
            {
                var elCaracter = texto[i];
                if (char.IsUpper(elCaracter))
                {
                    elConstructorDeTexto.Append('_');
                    elConstructorDeTexto.Append(char.ToLowerInvariant(elCaracter));
                }
                else
                {
                    elConstructorDeTexto.Append(elCaracter);
                }
            }

            return elConstructorDeTexto.ToString();
        }

        public static SqlParameter CrearEstructuradoDeUnaFila(
            string nombreDeParametro,
            string nombreDeTipo,
            object fila,
            IReadOnlyList<string> ordenDeColumnas)
        {
            if (fila == null)
            {
                throw new ArgumentNullException(nameof(fila));
            }

            if (ordenDeColumnas == null || ordenDeColumnas.Count == 0)
            {
                throw new ArgumentException("columnOrder no puede ser vacío.", nameof(ordenDeColumnas));
            }

            var laTabla = new DataTable();

            foreach (var laColumna in ordenDeColumnas)
            {
                laTabla.Columns.Add(laColumna);
            }

            var losValores = new object[ordenDeColumnas.Count];
            var elTipoDeObjeto = fila.GetType();

            for (int i = 0; i < ordenDeColumnas.Count; i++)
            {
                var laPropiedadDeColumna = elTipoDeObjeto.GetProperty(ordenDeColumnas[i], BindingFlags.Public | BindingFlags.Instance);
                if (laPropiedadDeColumna == null)
                {
                    throw new ArgumentException($"La propiedad '{ordenDeColumnas[i]}' no existe en el objeto provisto.");
                }

                var elValorDeColumna = laPropiedadDeColumna.GetValue(fila);
                losValores[i] = elValorDeColumna ?? DBNull.Value;
            }

            laTabla.Rows.Add(losValores);

            return new SqlParameter(nombreDeParametro, SqlDbType.Structured)
            {
                TypeName = nombreDeTipo,
                Value = laTabla
            };
        }
    }
}