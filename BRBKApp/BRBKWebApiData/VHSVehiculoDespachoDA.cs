using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BRBKWebApiData
{
    [Serializable]
    public class VHSVehiculoDespachoDA : Base
    {
        private static string _dbname = "N4Middleware";

        public VHSVehiculoDespachoDA() : base()
        {
        }

        /// <summary>
        /// Inicializa la conexión SQL y los parámetros.
        /// </summary>
        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        /// <summary>
        /// Ejecuta el SP [cedi].[crear_EvidenciaEntrega_foto].
        /// </summary>
        /// <param name="vehiculoDespachadoId">ID del vehículo despachado.</param>
        /// <param name="ruta">Ruta de la foto (puede ser vacía).</param>
        /// <param name="observacion">Observaciones.</param>
        /// <param name="usuario">Usuario que registra.</param>
        /// <param name="OnError">Mensaje de error si ocurre.</param>
        public static int RegistraEvidenciaEntrega(
      long vehiculoDespachadoId,
      string ruta,
      string observacion,
      string usuario,
      out string OnError)
        {
            OnError = string.Empty;

            try
            {
                OnInit(_dbname);

                using (var connection = new SqlConnection(nueva_conexion))
                using (var command = new SqlCommand("[cedi].[crear_EvidenciaEntrega_foto]", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@VehiculoDespachadoID", vehiculoDespachadoId);
                    command.Parameters.AddWithValue("@i_ruta", ruta ?? string.Empty);
                    command.Parameters.AddWithValue("@Observacion", observacion ?? string.Empty);
                    command.Parameters.AddWithValue("@Usuario", usuario ?? "sistema");

                    connection.Open();
                    var result = command.ExecuteScalar();
                    connection.Close();

                    int idGenerado = result != null && result != DBNull.Value
                        ? Convert.ToInt32(result)
                        : 0;

                    return idGenerado;
                }
            }
            catch (Exception ex)
            {
                OnError = $"Error al registrar evidencia: {ex.Message}";
                return 0;
            }
        }

    }
}
