using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;


namespace SqlConexion
{
    public class Cls_Conexion
    {
        private static Int64? lm = -3;
        private string conexion_Local;
        private static string clase;

        //private string nueva_conexion;

        public string Conexion_Local {
            get { return this.conexion_Local; }
        }

        //get class name
        private static void init()
        {
            if (string.IsNullOrEmpty(clase))
                clase = MethodBase.GetCurrentMethod().DeclaringType.Name;
        }


        private static Cls_Conexion instancia = null;


        #region "Metodos de Conexion"

            public static Cls_Conexion Conexion()
            {
                if (instancia == null)
                    instancia = new Cls_Conexion();
                return instancia;
            }

            private Cls_Conexion()
            {
                if (string.IsNullOrEmpty(conexion_Local) && System.Configuration.ConfigurationManager.ConnectionStrings["BRBK"] != null)
                {
                    conexion_Local = System.Configuration.ConfigurationManager.ConnectionStrings["BRBK"].ConnectionString;
                    var s = TestServer();
                    if (!string.IsNullOrEmpty(s))
                    {
                        throw new ApplicationException(s);
                    }
                }
                else
                {
                    throw new ApplicationException("La cadena de conexión BRBK, no esta presente en el archivo de configuración");
                }
            }

        public static string Nueva_Conexion(string pTipo)
        {
            try
            {
                var _nueva_conexion = Cls_ExtraeParametros.Get_Parametro(pTipo);
                return _nueva_conexion.valor;
            }
            catch
            {
                return string.Empty;
            }


        }

        #endregion

        public string TestServer(string comando = null)
        {
            using (SqlConnection c = new SqlConnection(this.conexion_Local))
            {
                try
                {
                    c.Open();
                    var xc = c.CreateCommand();
                    xc.CommandText = string.IsNullOrEmpty(comando) ? "SELECT 1" : comando;
                    xc.ExecuteScalar();
                    xc.Dispose();
                    return string.Empty;
                }
                catch (SqlException e)
                {
                    return SQLControl(e);
                }
                catch (Exception e)
                {
                    if (c.State == ConnectionState.Open) { c.Close(); }
                    return e.Message;
                }
            }
        }

        private static string SQLControl(SqlException s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SqlError ei in s.Errors)
            {
                sb.AppendLine(string.Format("{0}, {1}, {2}, {3}", ei.LineNumber, ei.Message, ei.Procedure, ei.Server));
            }
            return sb.ToString();
        }

        #region "Metodos"

            public SqlParameter[] parseComands(Dictionary<string, object> plist)
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

            public List<T> ExecuteSelectControl<T>(string conection, int timeout, string pc_name, Dictionary<string, object> parameters, out string OError)
            {

                var lx = new List<DbDataRecord>();
                SqlCommand sp_com = new SqlCommand();
                var sql = new SqlConnection(conection);
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
                        OError = string.Empty;
                        if (re.HasRows)
                        {
                            return DataReaderToToList<T>(re);
                        }
                        return new List<T>();
                    }
                    catch (SqlException e)
                    {
                       
                        lm = LogEvent<SqlException>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        OError = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                        return null;
                    }
                    catch (Exception e)
                    {
                       
                        lm = LogEvent<Exception>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        OError = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                        return null;
                    }
                    finally
                    {
                        sp_com.Dispose();
                        if (sql.State == ConnectionState.Open)
                            sql.Close();
                    }

                }
            }

            public T ExecuteSelectOnly<T>(string conection, int timeout, string pc_name, Dictionary<string, object> parameters) where T : class
            {
                /* NULL, hubo un error / o no encontrado   */

                SqlCommand sp_com = new SqlCommand();
                var sql = new SqlConnection(conection);
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
                            return DataReaderToObject<T>(re);
                        }
                        return null;
                    }
                    catch (SqlException e)
                    {

                    //lm = BDTraza.LogEvent<SqlException>("SQL", nameof(ExecuteSelectOnly), pc_name, -2, clase, parameters, e.GetType().Name, e);
                        lm = LogEvent<SqlException>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        return null;
                    }
                    catch (Exception e)
                    {
                        //lm = BDTraza.LogEvent<Exception>("SQL", nameof(ExecuteSelectOnly), pc_name, 0, clase, parameters, e.GetType().Name, e);
                        lm = LogEvent<Exception>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        return null;
                    }
                    finally
                    {
                        sp_com.Dispose();
                        if (sql.State == ConnectionState.Open)
                            sql.Close();
                    }

                }
            }

            public Cls_Mensaje ExecuteSelectOnly(string conection, int timeout, string pc_name, Dictionary<string, object> parameters)
            {

            var lx = new List<DbDataRecord>();
            SqlCommand sp_com = new SqlCommand();
            var sql = new SqlConnection(conection);
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
                        return new Cls_Mensaje() { codigo = re.GetInt32(0), mensaje = re.GetString(1) };
                    }
                    return new Cls_Mensaje() { codigo = -1, mensaje = "La búsqueda no obtuvo resultados, cambie los parámetros y reintente" };
                }
                catch (SqlException e)
                {
                    lm = LogEvent<SqlException>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                    return new Cls_Mensaje() { codigo = -2, mensaje = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2)) };
                }
                catch (Exception e)
                {
                    lm = LogEvent<Exception>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                    return new Cls_Mensaje() { codigo = -2, mensaje = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2)) };
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }

            }
        }

            public Int64? ExecuteInsertUpdateDeleteReturn(string conection, int timeout, string pc_name, Dictionary<string, object> parameters, Action<string> control_error)
            {
                //for sql ->  select @@identity in normal transaction;

                SqlCommand sp_com = new SqlCommand();
                var sql = new SqlConnection(conection);
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
                        var t = sp_com.ExecuteScalar() as Int64?;
                        return t;
                    }
                    catch (SqlException e)
                    {

                        lm = LogEvent<SqlException>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        control_error?.Invoke(string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2)));
                        return null;

                   
                    }
                    catch (Exception e)
                    {
                        lm = LogEvent<Exception>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        control_error?.Invoke(string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2)));//n
                        return null;
                    }
                    finally
                    {
                        sp_com.Dispose();
                        if (sql.State == ConnectionState.Open)
                            sql.Close();
                    }
                }
            }

            public Int64? ExecuteInsertUpdateDeleteReturn(string conection, int timeout, string pc_name, Dictionary<string, object> parameters, out string control_error)
            {
               
                SqlCommand sp_com = new SqlCommand();
                var sql = new SqlConnection(conection);
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
                        var t = sp_com.ExecuteScalar() as Int64?;
                        control_error = string.Empty;
                        return t;
                    }
                    catch (SqlException e) {

                        lm = LogEvent<SqlException>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        control_error = string.Format("{0} {1}",string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                        return null;
                    }
                    catch (Exception e)
                    {
                   
                        lm = LogEvent<Exception>("SQL", nameof(ExecuteInsertUpdateDeleteReturn), pc_name, false, null, parameters, null, e);
                        control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                        
                        return null;
                    }
                    finally
                    {
                        sp_com.Dispose();
                        if (sql.State == ConnectionState.Open)
                            sql.Close();
                    }
                }
            }

        public int? ExecuteInsertUpdateDelete(string conection, int timeout, string pc_name, Dictionary<string, object> parameters, out string control_error)
        {
            control_error = string.Empty;
            SqlCommand sp_com = new SqlCommand();
            var sql = new SqlConnection(conection);
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
                    //insert = -1, up/del rw afectted
                    var t = sp_com.ExecuteNonQuery();

                    //si se insertó entonces continue
                    if (t < 0) { t = 1; }
                    return t;
                }
                catch (SqlException e)
                {
                    lm = LogEvent<SqlException>("SQL", nameof(ExecuteInsertUpdateDelete), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                    return null;
                }
                catch (Exception e)
                {
                    lm = LogEvent<Exception>("SQL", nameof(ExecuteInsertUpdateDelete), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                    
                    return null;
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }

        public bool? ExecuteSelectOnlyBool(string conection, int timeout, string pc_name, Dictionary<string, object> parameters, out string control_error)
        {

            SqlCommand sp_com = new SqlCommand();
            var sql = new SqlConnection(conection);
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
                    var t = sp_com.ExecuteScalar() as bool?;
                    control_error = string.Empty;
                    return t;
                }
                catch (SqlException e)
                {

                    lm = LogEvent<SqlException>("SQL", nameof(ExecuteSelectOnlyBool), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                    return null;
                }
                catch (Exception e)
                {

                    lm = LogEvent<Exception>("SQL", nameof(ExecuteSelectOnlyBool), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));

                    return null;
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }

        public string ExecuteSelectOnlyString(string conection, int timeout, string pc_name, Dictionary<string, object> parameters, out string control_error)
        {

            SqlCommand sp_com = new SqlCommand();
            var sql = new SqlConnection(conection);
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
                    var t = sp_com.ExecuteScalar() as string;
                    control_error = string.Empty;
                    return t;
                }
                catch (SqlException e)
                {

                    lm = LogEvent<SqlException>("SQL", nameof(ExecuteSelectOnlyString), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                    return null;
                }
                catch (Exception e)
                {

                    lm = LogEvent<Exception>("SQL", nameof(ExecuteSelectOnlyString), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));

                    return null;
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }

        public int? ExecuteSelectOnlyInt(string conection, int timeout, string pc_name, Dictionary<string, object> parameters, out string control_error)
        {

            SqlCommand sp_com = new SqlCommand();
            var sql = new SqlConnection(conection);
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
                    var t = sp_com.ExecuteScalar() as int?;
                    control_error = string.Empty;
                    return t;
                }
                catch (SqlException e)
                {

                    lm = LogEvent<SqlException>("SQL", nameof(ExecuteSelectOnlyBool), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));
                    return null;
                }
                catch (Exception e)
                {

                    lm = LogEvent<Exception>("SQL", nameof(ExecuteSelectOnlyBool), pc_name, false, null, parameters, null, e);
                    control_error = string.Format("{0} {1}", string.Format("Excepcion no.{0}", lm), string.Format("Reporte el número el siguiente ticket de servicio:{0}", lm.HasValue ? lm : -2));

                    return null;
                }
                finally
                {
                    sp_com.Dispose();
                    if (sql.State == ConnectionState.Open)
                        sql.Close();
                }
            }
        }

        #endregion

        #region "Mapeo de Metodos"

        private List<T> DataReaderToToList<T>(IDataReader dr)
                {
                    List<T> list = new List<T>();
                    var columns = Enumerable.Range(0, dr.FieldCount)
                                .Select(dr.GetName)
                                .ToList();
                    T obj = default(T);
                    while (dr.Read())
                    {
                        obj = Activator.CreateInstance<T>();
                        foreach (PropertyInfo prop in obj.GetType().GetProperties())
                        {
                            if (columns.FindIndex(s => s.ToLower().Equals(prop.Name.ToLower())) >= 0 && !object.Equals(dr[prop.Name.ToLower()], DBNull.Value))
                            {
                                prop.SetValue(obj, dr[prop.Name], null);
                            }
                        }
                        list.Add(obj);
                    }
                    return list;
                }

            private T DataReaderToObject<T>(IDataReader dr)
            {
                var columns = Enumerable.Range(0, dr.FieldCount)
                        .Select(dr.GetName)
                        .ToList();
                T obj = default(T);
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (columns.FindIndex(s => s.ToLower().Equals(prop.Name.ToLower())) >= 0 && !object.Equals(dr[prop.Name.ToLower()], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                return obj;
            }

        #endregion



        #region "Trace"
        public static Int64? LogEvent<T>(string _user, string _source, string _method, bool _result, string _object = null, Dictionary<string, object> _parameter_input = null, string _result_execution_message = null, T _exception = null) where T : System.Exception
        {
            try
            {
                string conn = System.Configuration.ConfigurationManager.ConnectionStrings["BRBK"]?.ConnectionString;
                if (string.IsNullOrEmpty(conn))
                {
                    return -2;
                }
                Int64 result = 0;
                conn = string.Format("{0};Enlist=\"false\"", conn);

                using (var conexion = new SqlConnection(conn))
                {

                    try
                    {
                        using (var comando = conexion.CreateCommand())
                        {
                            comando.CommandType = CommandType.StoredProcedure;
                            comando.CommandText = "[brbk].[insertLog]";
                            comando.Parameters.AddWithValue("@user", _user);
                            comando.Parameters.AddWithValue("@source", _source);
                            comando.Parameters.AddWithValue("@method", _method);
                            comando.Parameters.AddWithValue("@result", _result);
                            comando.Transaction = null;

                            //Objeto
                            if (!string.IsNullOrEmpty(_object))
                            {
                                comando.Parameters.AddWithValue("@object", _object);
                            }
                            //parametros
                            if (_parameter_input != null && _parameter_input.Count > 0)
                            {
                                StringBuilder sbb = new StringBuilder();
                                foreach (var i in _parameter_input)
                                {
                                    sbb.Append(string.Format("{0}={1}, ", i.Key, i.Value?.ToString()));
                                }
                                comando.Parameters.AddWithValue("@parameter_string", _result);
                            }
                            if (_exception != null)
                            {
                                var s = _exception as SqlException;
                                if (_exception.GetType() == typeof(SqlException) && s != null)
                                {
                                    comando.Parameters.AddWithValue("@primary_exception_message", SQLControl(s));
                                }
                                else
                                {
                                    comando.Parameters.AddWithValue("@primary_exception_message", _exception.Message);
                                }
                                comando.Parameters.AddWithValue("@secondary_exception_message", _exception.InnerException != null ? _exception.InnerException.Message : null);
                            }
                            if (!string.IsNullOrEmpty(_result_execution_message))
                            {
                                comando.Parameters.AddWithValue("@result_execution_message", _result_execution_message);
                            }
                            if (conexion.State == ConnectionState.Closed) { conexion.Open(); }
                            result = Int64.Parse(comando.ExecuteScalar().ToString());
                            conexion.EnlistTransaction(null);
                           
                            conexion.Close();
                            return result;
                        }
                    }
                    catch 
                    {
                        return null;
                    }
                    finally
                    {
                        if (conexion.State == ConnectionState.Open)
                        {
                            conexion.Close();
                        }
                        conexion.Dispose();
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
