using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace BRBKApp.Converters
{
    public class FormatLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Verificar si el valor es nulo o no es una cadena
            if (value == null || !(value is string mensaje) || string.IsNullOrWhiteSpace(mensaje))
            {
                return new List<object>(); // Retorna una lista vacía en caso de error
            }

            // Dividir por saltos de línea y filtrar líneas vacías
            var pares = mensaje.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(linea =>
                {
                    var partes = linea.Split(new[] { ':' }, 2); // Limita a 2 partes para evitar problemas
                    string key = partes.Length > 0 ? partes[0].Trim() : "";
                    string val = partes.Length > 1 ? partes[1].Trim() : "";
                    return new { Key = key, Value = val };
                }).ToList();

#if DEBUG
            System.Diagnostics.Debug.WriteLine("Pares generados: " + string.Join(", ", pares.Select(p => $"{p.Key}: {p.Value}")));
#endif

            return pares;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}