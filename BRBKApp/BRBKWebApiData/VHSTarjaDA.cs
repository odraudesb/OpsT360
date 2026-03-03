using ApiModels.AppModels;
using ApiModels.Parametros;
using SqlConexion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BRBKWebApiData
{
    [Serializable]
    public class VHSTarjaDA : Base
    {
        private static string _dbname = "N4Middleware";
        public VHSTarjaDA() : base()
        {

        }
        private static void OnInit(string Base)
        {
            sql_puntero = (sql_puntero == null) ? Cls_Conexion.Conexion() : sql_puntero;
            parametros = new Dictionary<string, object>();
            nueva_conexion = Cls_Conexion.Nueva_Conexion(Base);
        }
        public static List<VHSTarjaMensaje> GetTarja(bool filtrar, int idOrden, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();
            parametros.Add("filtrar", filtrar);
            parametros.Add("OrdenTrabajoId", idOrden);
            List<VHSTarjaMensaje> list = sql_puntero.ExecuteSelectControl<VHSTarjaMensaje>(nueva_conexion, 4000, "[vhs].lista_tarjas_pendientes", parametros, out OnError);

            return list;
        }

        public static int CrearNovedad(long detalleTarjaId, int TipoNovedadID, string descripcion, string usuario, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();
            parametros.Add("DetalleTarjaID", detalleTarjaId);
            parametros.Add("TipoNovedadID", TipoNovedadID);
            parametros.Add("Descripcion", descripcion);
            parametros.Add("Usuario", usuario);


            //var result = await cmd.ExecuteScalarAsync();

            var novedadId = sql_puntero.ExecuteSelectOnlyInt(
                nueva_conexion,
                4000, 
                "[vhs].[crear_novedad]",
                parametros,
                out OnError);

            return novedadId ?? -1;
        }

        public static int CrearNovedadFoto(int novedadId, string rutaFoto, string usuario, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();
            parametros.Add("NovedadID", novedadId);
            parametros.Add("RutaFoto", rutaFoto);
            parametros.Add("Usuario", usuario);

            var fotoId = sql_puntero.ExecuteSelectOnlyInt(
                nueva_conexion,
                4000,
                "[vhs].[crear_novedad_foto]",
                parametros,
                out OnError);

            return fotoId ?? -1;
        }

        public static List<VHSMensajeSimple> GetVehiculosDespacho(long paseId, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();
            parametros.Add("PaseID", paseId);

            List<VHSMensajeSimple> list = sql_puntero.ExecuteSelectControl<VHSMensajeSimple>(
                nueva_conexion,
                4000,
                "[vhs].[lista_vehiculos_despacho]",
                parametros,
                out OnError
            );

            return list;
        }


        public static VHSTarjaModel RegistraTarjaConFotos(VHSTarjaModel tarjaModel, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();
            parametros.Add("OrdenTrabajoId", tarjaModel.OrdenTrabajoId);
            parametros.Add("Observacion", tarjaModel.Observacion);
            parametros.Add("Contenido", tarjaModel.Contenido);
            parametros.Add("Usuario", tarjaModel.Usuario);
            var entero = sql_puntero.ExecuteSelectOnlyInt(nueva_conexion, 4000, "[vhs].[crear_tarja]", parametros, out OnError);
            if (!entero.HasValue)
            {
                return new VHSTarjaModel();
            }
            if (!((entero ?? 0) > 0))
            {
                return new VHSTarjaModel();
            }
            tarjaModel.TarjaId = entero.Value;
            foreach (var item in tarjaModel.TarjaFotos)
            {
                if (item.Ruta == string.Empty)
                {
                    continue;
                }
                item.TarjaId = entero ?? -1;
                parametros.Clear();
                parametros.Add("TarjaId", item.TarjaId);
                parametros.Add("i_ruta", item.Ruta);
                parametros.Add("Usuario", tarjaModel.Usuario);
                var detalleid = sql_puntero.ExecuteSelectOnlyInt(nueva_conexion, 4000, "[vhs].[crear_tarja_foto]", parametros, out OnError);
                if (detalleid.HasValue && (detalleid ?? 0) > 0)
                {
                    item.TarjaFotoId = detalleid.Value;
                }
            }
            //tarjaModel.OrdenTrabajoId
            var returnValue = tarjaModel;
            return returnValue;
        }

        public static bool? GetExigeFoto(string _code, out string OnError)
        {
            OnInit("N4Middleware");
            parametros.Clear();
            parametros.Add("i_code", _code);
            var obj = sql_puntero.ExecuteSelectOnlyBool(nueva_conexion, 4000, "seal.exigeFoto", parametros, out OnError);
            return obj;
        }

        public static List<VHSTarjaMensaje> GetTarjaConDetalle(bool filtrar, int idOrden, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();
            parametros.Add("filtrar", filtrar);
            parametros.Add("OrdenTrabajoId", idOrden);
            List<VHSTarjaMensaje> list = sql_puntero.ExecuteSelectControl<VHSTarjaMensaje>(nueva_conexion, 4000, "[vhs].lista_tarjas_pendientes", parametros, out OnError);
            foreach (var item in list)
            {
                parametros.Clear();
                parametros.Add("TarjaID", item.TarjaId);
                List<VHSTarjaDetalleMensaje> detalle = sql_puntero.ExecuteSelectControl<VHSTarjaDetalleMensaje>(nueva_conexion, 4000, "[vhs].[lista_detalle_tarjas]", parametros, out OnError);
                item.Detalle = detalle;
            }
            return list;
        }

        public static List<VHSTarjaDetalleMensaje> GetDetalleTarja(int idTarja, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();
            parametros.Add("TarjaID", idTarja);
            List<VHSTarjaDetalleMensaje> detalle = sql_puntero.ExecuteSelectControl<VHSTarjaDetalleMensaje>(nueva_conexion, 4000, "[vhs].[lista_detalle_tarjas]", parametros, out OnError);
            return detalle;
        }

        public static VHSTarjaDetalleMensaje AddTarjaDetalleConFoto(ParametroVHSTarjaDetalleAdd parametro, out string OnError)
        {
            OnInit(_dbname);
            parametros.Clear();

            parametros.Add("TarjaId", parametro.TarjaID);
            parametros.Add("TipoCargaDescripcion", parametro.TipoCargaDescripcion);
            parametros.Add("InformacionVehiculo", parametro.InformacionVehiculo);
            parametros.Add("DocumentoTransporte", parametro.DocumentoTransporte);
            parametros.Add("PackingList", parametro.PackingList);
            parametros.Add("VIN", parametro.VIN);
            parametros.Add("NumeroMotor", parametro.NumeroMotor);
            parametros.Add("UbicacionBodega", parametro.UbicacionBodega);
            parametros.Add("Observaciones", parametro.Observaciones);
            // parametros.Add("Usuario", parametro.Create_user);
            parametros.Add("Usuario", "qw");

            // Crear e inicializar SqlConnection usando la cadena de conexión
            using (var connection = new SqlConnection(nueva_conexion))
            using (var command = new SqlCommand("[vhs].[crear_detalle_tarja]", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                foreach (var param in parametros)
                {
                    command.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                }
                connection.Open();
                var result = command.ExecuteScalar(); // Captura el valor devuelto
                connection.Close();

                var entero = result != null && result != DBNull.Value ? Convert.ToInt32(result) : (int?)null;
                if (!entero.HasValue)
                {
                    OnError = "Error: No se obtuvo un valor válido para entero.";
                    return new VHSTarjaDetalleMensaje();
                }
                if (!((entero ?? 0) > 0))
                {
                    OnError = "Error: El valor de entero no es mayor a 0.";
                    return new VHSTarjaDetalleMensaje();
                }

                VHSTarjaDetalleMensaje entry = new VHSTarjaDetalleMensaje();
                entry.TarjaID = parametro.TarjaID;
                entry.DetalleTarjaID = entero.Value;
                foreach (var item in parametro.Fotos)
                {
                    if (item.FotosVehiculo == string.Empty)
                    {
                        continue;
                    }
                    item.DetalleTarjaID = entry.DetalleTarjaID;
                    parametros.Clear();
                    parametros.Add("DetalleTarjaId", item.DetalleTarjaID);
                    parametros.Add("i_ruta", item.FotosVehiculo);
                    parametros.Add("Usuario", parametro.Create_user);
                    using (var fotoConnection = new SqlConnection(nueva_conexion))
                    using (var fotoCommand = new SqlCommand("[vhs].[crear_detalle_tarja_foto]", fotoConnection))
                    {
                        fotoCommand.CommandType = CommandType.StoredProcedure;
                        foreach (var param in parametros)
                        {
                            fotoCommand.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                        }
                        fotoConnection.Open();
                        var detalleid = fotoCommand.ExecuteScalar(); // Captura el valor devuelto
                        fotoConnection.Close();

                        if (detalleid != null && detalleid != DBNull.Value)
                        {
                            item.FotoID = Convert.ToInt32(detalleid);
                        }
                    }
                }
                entry.Fotos = parametro.Fotos;
                List<VHSTarjaDetalleMensaje> detalle = GetDetalleTarja(entry.TarjaID, out OnError);
                entry.Mensaje = detalle.First(e => e.DetalleTarjaID == entry.DetalleTarjaID).Mensaje;
                OnError = string.Empty; // Asignar un valor por defecto al salir
                var returnValue = entry;
                return returnValue;
            }
        }


        public static AppModelVHSTarjaDetalle GetTarjaDetalleById(long detalleTarjaId, out string OnError)
        {
            OnInit(_dbname);
            OnError = string.Empty;
            AppModelVHSTarjaDetalle entry = new AppModelVHSTarjaDetalle();

            try
            {
                using (var connection = new SqlConnection(nueva_conexion))
                using (var command = new SqlCommand("[vhs].[consultar_detalle_tarja]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DetalleTarjaId", detalleTarjaId);
                    Console.WriteLine($"Parámetro @DetalleTarjaId enviado: {detalleTarjaId}");
                    Console.WriteLine($"Conexión utilizada: {nueva_conexion}");

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Usar GetInt32 para DetalleTarjaID
                            if (reader["DetalleTarjaID"] != DBNull.Value)
                            {
                                entry.DetalleTarjaID = reader.GetInt32(reader.GetOrdinal("DetalleTarjaID"));
                            }
                            else
                            {
                                entry.DetalleTarjaID = 0; // Valor por defecto si es NULL
                            }

                            // Usar GetInt32 para TarjaID (asumiendo que también es int)
                            if (reader["TarjaID"] != DBNull.Value)
                            {
                                entry.TarjaID = reader.GetInt32(reader.GetOrdinal("TarjaID"));
                            }
                            else
                            {
                                entry.TarjaID = 0;
                            }

                            entry.TipoCargaDescripcion = reader["TipoCargaDescripcion"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("TipoCargaDescripcion")) : null;
                            entry.InformacionVehiculo = reader["InformacionVehiculo"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("InformacionVehiculo")) : null;
                            entry.DocumentoTransporte = reader["DocumentoTransporte"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("DocumentoTransporte")) : null;
                            entry.PackingList = reader["PackingList"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("PackingList")) : null;
                            entry.VIN = reader["VIN"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("VIN")) : null;
                            entry.NumeroMotor = reader["NumeroMotor"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("NumeroMotor")) : null;
                            entry.Observaciones = reader["Observaciones"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("Observaciones")) : null;
                            entry.nombre_bloque = reader["nombre_bloque"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("nombre_bloque")) : null;

                            // Nuevos campos: Bloque y NumeroBloque
                            // Nuevos campos: Bloque y NumeroBloque
                            entry.Id = reader["Id"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("Id")) : 0;
                            if (reader["NumeroBloque"] != DBNull.Value)
                            {
                                entry.NumeroBloque = reader.GetInt32(reader.GetOrdinal("NumeroBloque"));
                            }
                            else
                            {
                                entry.NumeroBloque = 0; // Valor por defecto si es NULL
                            }


                            if (reader["NumeroBloque"] != DBNull.Value)
                            {
                                entry.NumeroBloque = reader.GetInt32(reader.GetOrdinal("NumeroBloque"));
                            }
                            else
                            {
                                entry.NumeroBloque = 0; // Valor por defecto si es NULL
                            }

                            if (reader["Fotos"] != DBNull.Value)
                            {
                                var fotosJson = reader.GetString(reader.GetOrdinal("Fotos"));
                                entry.Fotos = JsonConvert.DeserializeObject<List<VHSTarjaDetalleFoto>>(fotosJson);
                            }
                        }
                        else
                        {
                            OnError = "No se encontraron datos para el DetalleTarjaId proporcionado.";
                            Console.WriteLine(OnError);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                OnError = $"Error al consultar detalle de tarja: {ex.Message}";
                return new AppModelVHSTarjaDetalle();
            }

            return entry;
        }
        public static bool UpdateTarjaDetalleBloque(long detalleTarjaId, int bloqueId, int numeroBloque, string VIN, string Observaciones, out string OnError)
        {
            OnInit(_dbname);
            OnError = string.Empty;

            try
            {
                using (var connection = new SqlConnection(nueva_conexion))
                using (var command = new SqlCommand("[vhs].[actualizar_detalle_tarja]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DetalleTarjaId", detalleTarjaId);
                    command.Parameters.AddWithValue("@BloqueID", bloqueId);
                    command.Parameters.AddWithValue("@NumeroBloque", numeroBloque);
                    command.Parameters.AddWithValue("@VIN", VIN);
                    command.Parameters.AddWithValue("@Observaciones", Observaciones);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        return true;
                    }
                    else
                    {
                        OnError = "No se actualizó ningún registro para el DetalleTarjaId proporcionado.";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                OnError = $"Error al actualizar detalle de tarja: {ex.Message}";
                return false;
            }
        }
        public static List<Dictionary<string, object>> GetBloques(out string OnError)
        {
            OnInit(_dbname);
            OnError = string.Empty;
            var parametros = new Dictionary<string, object>();

            try
            {
                using (var connection = new SqlConnection(nueva_conexion))
                using (var command = new SqlCommand("[vhs].[lista_bloques]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    var results = new List<Dictionary<string, object>>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>
                    {
                        { "id", reader["id"] },
                        { "nombre_bloque", reader["nombre_bloque"] },
                        { "capacidad", reader["capacidad"] }
                    };
                            results.Add(row);
                        }
                    }
                    connection.Close();
                    return results;
                }
            }
            catch (Exception ex)
            {
                OnError = $"Error al obtener la lista de bloques: {ex.Message}";
                return new List<Dictionary<string, object>>();
            }
        }

        public static List<Dictionary<string, object>> GetTiposNovedad(out string OnError)
        {
            OnInit(_dbname);
            OnError = string.Empty;

            try
            {
                using (var connection = new SqlConnection(nueva_conexion))
                using (var command = new SqlCommand("[vhs].[lista_tipos_novedad]", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    var results = new List<Dictionary<string, object>>();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>
                    {
                        { "TipoNovedadID", reader["TipoNovedadID"] },
                        { "Nombre", reader["Nombre"] },
                        { "Descripcion", reader["Descripcion"] }
                    };
                            results.Add(row);
                        }
                    }
                    connection.Close();
                    return results;
                }
            }
            catch (Exception ex)
            {
                OnError = $"Error al obtener tipos de novedad: {ex.Message}";
                return new List<Dictionary<string, object>>();
            }
        }

    }


}
