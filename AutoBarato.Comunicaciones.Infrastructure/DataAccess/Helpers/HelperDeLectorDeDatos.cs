using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Reflection;

namespace AutoBarato.Comunicaciones.Infrastructure.DataAccess.Helpers
{
    public static class HelperDeLectorDeDatos
    {
        public static List<T> MapearLista<T>(DbDataReader lector) where T : new()
        {
            var losElementosMapeados = new List<T>();
            var lasPropiedades = typeof(T).GetProperties();

            while (lector.Read())
            {
                var elObjeto = new T();

                foreach (var laPropiedad in lasPropiedades)
                {
                    // 🆕 Soporte para [Column("nombre_columna")]
                    var elNombreDeColumna = laPropiedad.GetCustomAttribute<ColumnAttribute>()?.Name ?? laPropiedad.Name;

                    if (!lector.TieneColumna(elNombreDeColumna))
                    {
                        continue;
                    }

                    var elValor = lector[elNombreDeColumna];

                    if (elValor == DBNull.Value)
                    {
                        continue;
                    }

                    try
                    {
                        var elTipoDestino = Nullable.GetUnderlyingType(laPropiedad.PropertyType) ?? laPropiedad.PropertyType;

                        if (elTipoDestino == typeof(decimal))
                        {
                            laPropiedad.SetValue(elObjeto, Convert.ToDecimal(elValor));
                        }
                        else if (elTipoDestino == typeof(double))
                        {
                            laPropiedad.SetValue(elObjeto, Convert.ToDouble(elValor));
                        }
                        else if (elTipoDestino == typeof(float))
                        {
                            laPropiedad.SetValue(elObjeto, Convert.ToSingle(elValor));
                        }
                        else
                        {
                            laPropiedad.SetValue(elObjeto, Convert.ChangeType(elValor, elTipoDestino));
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine($"❌ Error al asignar propiedad {columnName}: {ex.Message}");
                    }
                }

                losElementosMapeados.Add(elObjeto);
            }

            return losElementosMapeados;
        }

        public static bool TieneColumna(this DbDataReader lector, string nombreDeColumna)
        {
            for (int contador = 0; contador < lector.FieldCount; contador++)
            {
                if (lector.GetName(contador).Equals(nombreDeColumna, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}