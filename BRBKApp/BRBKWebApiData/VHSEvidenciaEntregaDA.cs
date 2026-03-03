using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace BRBKWebApiData
{
    [Serializable]
    public class VHSEvidenciaEntregaDA : Base
    {
        private static string _dbname = "N4Middleware";

        public VHSEvidenciaEntregaDA() : base() { }

        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }

        public static long CrearEvidenciaEntrega(long vehiculoDespachadoId, string observacion, string usuario, out string OnError)
        {
            OnInit(_dbname);
            OnError = string.Empty;

            using (var connection = new SqlConnection(nueva_conexion))
            using (var command = new SqlCommand("[cedi].[crear_EvidenciaEntrega]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@VehiculoDespachadoID", vehiculoDespachadoId);
                command.Parameters.AddWithValue("@Observacion", observacion ?? string.Empty);
                command.Parameters.AddWithValue("@Usuario", usuario ?? "sistema");

                connection.Open();
                var result = command.ExecuteScalar();
                connection.Close();

                long idGenerado = result != null && result != DBNull.Value
                    ? Convert.ToInt64(result)
                    : -1;

                return idGenerado;
            }
        }

        public static int CrearEvidenciaEntregaFoto(long evidenciaId, string ruta, string usuario, out string OnError)
        {
            OnInit(_dbname);
            OnError = string.Empty;

            using (var connection = new SqlConnection(nueva_conexion))
            using (var command = new SqlCommand("[cedi].[crear_evidencia_entrega_foto]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EvidenciaEntregaID", evidenciaId);
                command.Parameters.AddWithValue("@i_ruta", ruta ?? string.Empty);
                command.Parameters.AddWithValue("@Usuario", usuario ?? "sistema");

                connection.Open();
                var result = command.ExecuteScalar();
                connection.Close();

                int fotoId = result != null && result != DBNull.Value
                    ? Convert.ToInt32(result)
                    : -1;

                return fotoId;
            }
        }
    }
}
