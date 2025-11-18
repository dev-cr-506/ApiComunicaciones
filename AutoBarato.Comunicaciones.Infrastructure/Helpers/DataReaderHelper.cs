using  System;
using  System.Collections.Generic;
using  System.ComponentModel.DataAnnotations.Schema;
using  System.Data.Common;
using  System.Linq;
using  System.Reflection;
using  System.Text;
using  System.Threading.Tasks;

namespace AutoBarato.Comunicaciones.Infrastructure.Helpers
{
    public static class DataReaderHelper
    {
        public static List<T> MapList<T>(DbDataReader reader) where T : new()
        {
            var result = new List<T>();
            var props = typeof(T).GetProperties();

            // Mostrar columnas disponibles
            Console.WriteLine("🔍 Columnas en el resultado:");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Console.WriteLine($"- {reader.GetName(i)}");
            }

            while (reader.Read())
            {
                var obj = new T();

                foreach (var prop in props)
                {
                    // 🆕 Soporte para [Column("nombre_columna")]
                    var columnName = prop.GetCustomAttribute<ColumnAttribute>()?.Name ?? prop.Name;

                    if (!reader.HasColumn(columnName))
                    {
                        Console.WriteLine($"⚠️  Columna no encontrada: {columnName}");
                        continue;
                    }

                    var value = reader[columnName];

                    if (value == DBNull.Value)
                    {
                        Console.WriteLine($"🔸 Columna '{columnName}' está null");
                        continue;
                    }

                    try
                    {
                        prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType));
                        Console.WriteLine($"✅ Asignado: {columnName} = {value}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error al asignar propiedad {columnName}: {ex.Message}");
                    }
                }

                result.Add(obj);
            }

            Console.WriteLine($"✔ Total registros mapeados: {result.Count}");
            return result;
        }

        public static bool HasColumn(this DbDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }
    }
}
