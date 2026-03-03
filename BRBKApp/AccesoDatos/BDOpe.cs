using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AccesoDatos
{
    public static class BDOpe
    {
        private static string clase;
        public enum ParametroDireccion
        {
            Entrada, Salida, Ambos
        }
        private static Int64? lm = -3;
      //  private static SqlConnection sql = null;
        public static SqlParameter[] parseComands(Dictionary<string, object> plist)
        {
            if (plist == null || plist.Count <= 0)
            {
                return null;
            }
            SqlParameter[] colectionPars = new SqlParameter[plist.Count];
            var i = 0;
            foreach (var f in plist)
            {
                var pq = new SqlParameter();
                pq.Direction = ParameterDirection.Input;
                pq.ParameterName = string.Format("@{0}", f.Key);
                pq.Value = f.Value;
                colectionPars[i] = pq;
                i++;
            }
            return colectionPars;
        }
        public static SqlParameter[] parseComands(List<Tuple<string, object, ParametroDireccion>> plist)
        {
            if (plist == null || plist.Count <= 0)
            {
                return null;
            }
            SqlParameter[] colectionPars = new SqlParameter[plist.Count];
            var i = 0;
            foreach (var f in plist)
            {
                var pq = new SqlParameter();
                pq.Direction = f.Item3 == ParametroDireccion.Entrada ? ParameterDirection.Input : ParameterDirection.Output;
                pq.ParameterName = string.Format("@{0}", f.Item1);
                pq.Value = f.Item3 == ParametroDireccion.Entrada ? f.Item2 : DBNull.Value;
                colectionPars[i] = pq;
                i++;
            }
            return colectionPars;
        }
        public static DataOperacion<List<T>> ComandoSelectALista<T>(string conn, string pc_name, Dictionary<string, object> parameters, int timeout = 6000) where T : class, new()
        {
            init();
            var ob = new List<T>();
            SqlConnection sql = null;
            SqlCommand sp_com = new SqlCommand();
            sql = new SqlConnection(conn);
            sp_com.CommandType = CommandType.StoredProcedure;
            sp_com.Connection = sql;
            sp_com.CommandTimeout = timeout;
            sp_com.CommandText = pc_name;
            if (parameters != null && parameters.Count > 0)
            {
                sp_com.Parameters.AddRange(parseComands(parameters));
            }
            using (sql)
            {
                try
                {
                    sql.Open();
                    SqlDataReader re = sp_com.ExecuteReader(CommandBehavior.CloseConnection);
                    if (re.HasRows)
                    {
                        while (re.Read())
                        {
                            ob.Add(BDMap.ReaderAObjeto<T>(re));
                        }
                        return DataOperacion<List<T>>.CrearResultadoExitoso(ob, string.Format("Total filas {0}", ob.Count));
                    }
                    else
                    {
                        return DataOperacion<List<T>>.CrearFalla("No se encontraron registros que mostrar", "Cambie los parámetros");
                    }

                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoSelectALista), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<List<T>>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoSelectALista), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<List<T>>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -1));
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }

            }
        }
        public static DataOperacion<T> ComandoSelectAEntidad<T>(string conn, string pc_name, Dictionary<string, object> parameters, int timeout = 6000) where T : class, new()
        {
            init();
            SqlCommand sp_com = new SqlCommand();
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            sp_com.CommandType = CommandType.StoredProcedure;
            sp_com.Connection = sql;
            sp_com.CommandTimeout = timeout;
            sp_com.CommandText = pc_name;
            if (parameters != null && parameters.Count > 0)
            {
                sp_com.Parameters.AddRange(parseComands(parameters));
            }
            using (sql)
            {
                try
                {
                    sql.Open();
                    SqlDataReader re = sp_com.ExecuteReader(CommandBehavior.CloseConnection);
                    if (re.HasRows)
                    {
                        re.Read();
                        var ob = BDMap.ReaderAObjeto<T>(re);
                        return DataOperacion<T>.CrearResultadoExitoso(ob, "Correcto");

                    }
                    else
                    {
                        return DataOperacion<T>.CrearFalla("No se encontraron registros que mostrar", "Cambie los parámetros");
                    }
                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoSelectAEntidad), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoSelectAEntidad), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));

                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }

            }
        }
        public static DataOperacion<DataTable> ComadoSelectADatatable(string conn, string pc_name, Dictionary<string, object> parameters = null, int timeout = 6000)
        {
            init();
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            try
            {
                using (sql)
                {
                    using (var cmd = sql.CreateCommand())
                    {
                        cmd.CommandText = pc_name;
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null && parameters.Count > 0)
                        {
                            cmd.Parameters.AddRange(parseComands(parameters));
                        }
                        cmd.Connection.ConnectionString = sql.ConnectionString;
                        cmd.Connection.Open();
                        var table = new DataTable();
                        table.Load(cmd.ExecuteReader());
                        if (table == null || table.Rows.Count <= 0)
                        {
                            WarningException wa = new WarningException("La ejecución de proceso resulto en tabla nula o vacía");
                            lm = BDTraza.LogEvent<WarningException>("SQL", nameof(ComandoSelectAEntidad), pc_name, false, clase, parameters, wa.GetType().Name, wa);
                            return DataOperacion<DataTable>.CrearFalla("No se encontraron registros que mostrar", string.Format("Seguimiento No.{0}", lm));
                        }
                        return DataOperacion<DataTable>.CrearResultadoExitoso(table, "Correcto");
                    }
                }
            }
            catch (SqlException e)
            {
                lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComadoSelectADatatable), pc_name, false, clase, parameters, e.GetType().Name, e);
                return DataOperacion<DataTable>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
            }
            catch (Exception e)
            {
                lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComadoSelectADatatable), pc_name, false, clase, parameters, e.GetType().Name, e);
                return DataOperacion<DataTable>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
            }
        }
        public static DataOperacion<T> ComandoSelectEscalar<T>(string conn, string pc_name, Dictionary<string, object> parameters, int timeout = 6000) where T : struct
        {
            SqlCommand sp_com = new SqlCommand();
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            sp_com.CommandType = CommandType.Text;
            sp_com.Connection = sql;
            sp_com.CommandTimeout = timeout;
            sp_com.CommandText = pc_name;
            if (parameters != null && parameters.Count > 0)
            {
                sp_com.Parameters.AddRange(parseComands(parameters));
            }
            using (sql)
            {
                try
                {
                    sql.Open();
                    T ti;
                    var op = sp_com.ExecuteScalar();
                    if (op != null && op.GetType() != typeof(DBNull))
                    {
                        ti = (T)op;
                        return DataOperacion<T>.CrearResultadoExitoso(ti, "Correcto");
                    }
                    else
                    {
                        return DataOperacion<T>.CrearFalla("El procedimiento o función no retorna valores");
                    }
                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoSelectEscalar), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (InvalidCastException e)
                {
                    lm = BDTraza.LogEvent<InvalidCastException>("SQL", nameof(ComandoSelectEscalar), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }
        public static DataOperacion<Int32> ComandoInsertUpdateDeleteFila(string conn, string pc_name, Dictionary<string, object> parameters, int timeout = 6000)
        {
            init();
            SqlCommand sp_com = new SqlCommand();
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            sp_com.CommandType = CommandType.StoredProcedure;
            sp_com.Connection = sql;
            sp_com.CommandTimeout = timeout;
            sp_com.CommandText = pc_name;
            if (parameters != null && parameters.Count > 0)
            {
                sp_com.Parameters.AddRange(parseComands(parameters));
            }
            using (sql)
            {
                try
                {
                    sql.Open();
                    var ti = sp_com.ExecuteNonQuery();
                    //ti->filas afectadas insert,update, delete
                    // Si es cero no afecto ninguna, si es -1 fallo
                    return DataOperacion<Int32>.CrearResultadoExitoso(ti, string.Format("{0}", ti));
                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoInsertUpdateDeleteFila), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<Int32>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoInsertUpdateDeleteFila), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<Int32>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }
        public static DataOperacion<Int64?> ComandoInsertUpdateDeleteID(string conn, string pc_name, Dictionary<string, object> parameters, int timeout = 6000)
        {
            init();
            SqlCommand sp_com = new SqlCommand();
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            sp_com.CommandType = CommandType.StoredProcedure;
            sp_com.Connection = sql;
            sp_com.CommandTimeout = timeout;
            sp_com.CommandText = pc_name;
            if (parameters != null && parameters.Count > 0)
            {
                sp_com.Parameters.AddRange(parseComands(parameters));
            }
            using (sql)
            {
                try
                {
                    sql.Open();
                    var ti = sp_com.ExecuteScalar() as Int64?;
                    if (!ti.HasValue)
                    {
                        var et = new ApplicationException("Ejecución no retorna valor identidad");
                        lm = BDTraza.LogEvent<ApplicationException>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name, false, clase, parameters, et.GetType().Name, et);
                        return DataOperacion<Int64?>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                    }
                    return DataOperacion<Int64?>.CrearResultadoExitoso(ti, "Correcto");
                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<Int64?>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<Int64?>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }
        public static DataOperacion<Int64?> ComandoTransaccion(string conn, string pc_name, Dictionary<string, object> parameters, int timeout = 6000)
        {
            init();
            SqlCommand sp_com = new SqlCommand();
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            sp_com.CommandType = CommandType.StoredProcedure;
            sp_com.Connection = sql;
            sp_com.CommandTimeout = timeout;
            sp_com.CommandText = pc_name;
            if (parameters != null && parameters.Count > 0)
            {
                sp_com.Parameters.AddRange(parseComands(parameters));
            }
            using (sql)
            {
                try
                {
                    sql.Open();
                    var ti = sp_com.ExecuteScalar() as Int64?;
                    if (!ti.HasValue)
                    {
                        var et = new ApplicationException("Ejecución no retorna valor identidad");
                        lm = BDTraza.LogEvent<ApplicationException>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name, false, clase, parameters, et.GetType().Name, et);
                        return DataOperacion<Int64?>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                    }
                    return DataOperacion<Int64?>.CrearResultadoExitoso(ti, "Correcto");
                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name,false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<Int64?>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<Int64?>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }
        public static DataOperacion<T> ComandoSelectEscalarRef<T>(string conn, string pc_name, Dictionary<string, object> parameters, int timeout = 6000) where T : class
        {
            SqlCommand sp_com = new SqlCommand();
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            sp_com.CommandType = CommandType.Text;
            sp_com.Connection = sql;
            sp_com.CommandTimeout = timeout;
            sp_com.CommandText = pc_name;
            if (parameters != null && parameters.Count > 0)
            {
                sp_com.Parameters.AddRange(parseComands(parameters));
            }
            using (sql)
            {
                try
                {
                    sql.Open();
                    T ti;
                    var op = sp_com.ExecuteScalar();
                    if (op != null && op.GetType() != typeof(DBNull))
                    {
                        ti = (T)op;
                        return DataOperacion<T>.CrearResultadoExitoso(ti, "Correcto");
                    }
                    else
                    {
                       return DataOperacion<T>.CrearFalla("El procedimiento o función no retorna valores");
                    }
                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoSelectEscalar), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (InvalidCastException e)
                {
                    lm = BDTraza.LogEvent<InvalidCastException>("SQL", nameof(ComandoSelectEscalar), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoInsertUpdateDeleteID), pc_name, false, clase, parameters, e.GetType().Name, e);
                    return DataOperacion<T>.CrearFalla(string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }


        public static DataOperacion<Int32> ComandoInsertMasivo(string conn, string sqltable, DataTable bulkdata, int timeout = 6000)
        {
            SqlConnection sql = null;
            sql = new SqlConnection(conn);
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
            {
                bulkCopy.DestinationTableName = sqltable;
                try
                {
                    bulkCopy.WriteToServer(bulkdata);
                    return DataOperacion<Int32>.CrearResultadoExitoso(bulkdata.Rows.Count);
                }
                catch (SqlException e)
                {
                    lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ComandoSelectEscalar), "SQL_BULK", false, clase, null, e.GetType().Name, e);
                    return DataOperacion<Int32>.CrearFalla(string.Format("Ocurrio una excepcion SQL {0}",lm));
                }
                catch (InvalidCastException e)
                {
                    lm = BDTraza.LogEvent<InvalidCastException>("SQL", nameof(ComandoSelectEscalar), "SQL_BULK", false, clase, null, e.GetType().Name, e);
                    return DataOperacion<Int32>.CrearFalla(string.Format("Ocurrio una excepcion CONVERSION {0}", lm));
                }
                catch (Exception e)
                {
                    lm = BDTraza.LogEvent<Exception>("SQL", nameof(ComandoInsertUpdateDeleteID), "SQL_BULK", false, clase, null, e.GetType().Name, e);
                    return DataOperacion<Int32>.CrearFalla(string.Format("Ocurrio una excepcion GENERAL {0}", lm));
                }
                finally
                {
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }



        //get class name
        private static void init()
        {
            if (string.IsNullOrEmpty(clase))
            {
                clase = MethodBase.GetCurrentMethod().DeclaringType.Name;
            }

        }

     

    }
}
