using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AccesoDatos
{
    public class BDTraza
    {
        private static BDCon db = null;
        private static string cc_er = string.Empty;
        private static bool _errsql;
        private static void init()
        {
            db = new BDCon();
            _errsql  = db.SetBasica("BRBK").Exitoso;
        }
        //public static Int64? LogEvent<T>(string usuario, string origen, string metodo, int resultado, string clase = null, Dictionary<string, object> parametros = null, string resultado_ejecucion = null, T _exception = null) where T : System.Exception
        //{
        //    try
        //    {
        //        init();
        //        if (!_errsql) { return -2; }
        //        Int64 result = 0;
        //        string conn = db.CadenaBasica;
        //        conn = string.Format("{0};Enlist=\"false\"", conn);
        //        using (var conexion = new SqlConnection(conn))
        //        {
        //            try
        //            {
        //                using (var comando = conexion.CreateCommand())
        //                {
        //                    comando.CommandType = CommandType.StoredProcedure;
        //                    comando.CommandText = "[brbk].[insertLog]";
        //                    comando.Parameters.AddWithValue("@user", usuario);
        //                    comando.Parameters.AddWithValue("@source", origen);
        //                    comando.Parameters.AddWithValue("@method", metodo);
        //                    comando.Parameters.AddWithValue("@result", false);
        //                    comando.Transaction = null;

        //                    //Objeto
        //                    if (!string.IsNullOrEmpty(clase))
        //                    {
        //                        comando.Parameters.AddWithValue("@object", clase);
        //                    }
        //                    //parametros
        //                    if (parametros != null && parametros.Count > 0)
        //                    {
        //                        StringBuilder sbb = new StringBuilder();
        //                        sbb.Append("<parametros>");
        //                        foreach (var i in parametros)
        //                        {
        //                            if (IsXml(i.Value?.ToString()))
        //                            {
        //                                sbb.AppendFormat("<parametro nombre=\"{0}\" >{1}</parametro>",i.Key,CommentXml(  i.Value?.ToString()));
        //                            }
        //                            else
        //                            {
        //                                sbb.Append(string.Format("<parametro nombre=\"{0}\" valor=\"{1}\" />", i.Key, i.Value?.ToString()));
        //                            }


        //                        }
        //                        sbb.Append("</parametros>");
        //                        comando.Parameters.AddWithValue("@parameter_string", sbb.ToString());
        //                    }
        //                    if (_exception != null)
        //                    {
        //                        var s = _exception as SqlException;
        //                        if (_exception.GetType() == typeof(SqlException) && s != null)
        //                        {
        //                            comando.Parameters.AddWithValue("@primary_exception_message", BDCon.SQLControl(s));
        //                        }
        //                        else
        //                        {
        //                            comando.Parameters.AddWithValue("@primary_exception_message", _exception.Message);
        //                        }

        //                        comando.Parameters.AddWithValue("@secondary_exception_message", _exception.InnerException != null ? _exception.InnerException.Message : null);
        //                    }
        //                    if (!string.IsNullOrEmpty(resultado_ejecucion))
        //                    {
        //                        comando.Parameters.AddWithValue("@result_execution_message", resultado_ejecucion);
        //                    }
        //                    if (conexion.State == ConnectionState.Closed) { conexion.Open(); }
        //                    result = Int64.Parse(comando.ExecuteScalar().ToString());
        //                    conexion.EnlistTransaction(null);
        //                    conexion.Close();
        //                    return result;
        //                }
        //            }
        //            catch
        //            {
        //                return null;
        //            }
        //            finally
        //            {
        //                if (conexion.State == ConnectionState.Open)
        //                {
        //                    conexion.Close();
        //                }
        //                conexion.Dispose();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

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
                                comando.Parameters.AddWithValue("@parameter_string", sbb.ToString());
                            }
                            if (_exception != null)
                            {
                                var s = _exception as SqlException;
                                if (_exception.GetType() == typeof(SqlException) && s != null)
                                {
                                    comando.Parameters.AddWithValue("@primary_exception_message", BDCon.SQLControl(s));
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

                            var t = comando.ExecuteScalar() as Int64?;
                            result = long.Parse(t.ToString());
                            //result = Int64.Parse(comando.ExecuteScalar().ToString());
                            conexion.EnlistTransaction(null);

                            conexion.Close();
                            return result;
                        }
                    }
                    catch//(Exception ex)
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
        public static Int64? TraceEvent<T>(string usuario, string origen, string metodo, T objeto, string resultado_ejecucion = null) where T : class
        {
            try
            {
                init();
                if (!_errsql) { return -2; }
                Int64 result = 0;
                string conn = db.CadenaBasica;
                conn = string.Format("{0};Enlist=\"false\"", conn);
                using (var conexion = new SqlConnection(conn))
                {
                    try
                    {
                        using (var comando = conexion.CreateCommand())
                        {
                            comando.CommandType = CommandType.StoredProcedure;
                            comando.CommandText = "[brbk].[insertLog]";
                            comando.Parameters.AddWithValue("@user", usuario);
                            comando.Parameters.AddWithValue("@source", origen);
                            comando.Parameters.AddWithValue("@method", metodo);
                            comando.Parameters.AddWithValue("@resultado", true);
                            comando.Transaction = null;
                            //Objeto
                            if (objeto != null)
                            {
                                comando.Parameters.AddWithValue("@object", objeto?.GetType().Name);
                            }
                            //parametros
                            if (objeto != null)
                            {
                                var s = BDMap.Serializar<T>(objeto,out cc_er );
                                if (!string.IsNullOrEmpty(cc_er))
                                {
                                    s = cc_er;
                                }
                                comando.Parameters.AddWithValue("@parameter_string", s);
                            }
                            comando.Parameters.AddWithValue("@result_execution_message", resultado_ejecucion);
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
        public static Int64? TraceMove(string usuario, string origen, string clase, string metodo,  string resultado_ejecucion) 
        {
            try
            {
                init();
                if (!_errsql) { return -2; }
                Int64 result = 0;
                string conn = db.CadenaBasica;
                conn = string.Format("{0};Enlist=\"false\"", conn);
                using (var conexion = new SqlConnection(conn))
                {
                    try
                    {
                        using (var comando = conexion.CreateCommand())
                        {
                            comando.CommandType = CommandType.StoredProcedure;
                            comando.CommandText = "[brbk].[insertLog]";
                            comando.Parameters.AddWithValue("@user", usuario);
                            comando.Parameters.AddWithValue("@source", origen);
                            comando.Parameters.AddWithValue("@method", metodo);
                            comando.Parameters.AddWithValue("@result", false);
                            comando.Transaction = null;
                            //Objeto
                            comando.Parameters.AddWithValue("@object", clase);
                            //parametros

                            comando.Parameters.AddWithValue("@result_execution_message", resultado_ejecucion);
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
        public static bool IsXml(string input)
        {
            return (input.StartsWith("<") && input.EndsWith(">"));
        }
        private static string CommentXml(object xm)
        {
            //<![CDATA[         characters with markup  ]]>
            if ( xm != null  && IsXml(xm.ToString()))
            {
                return string.Format("<![CDATA[{0}]]>",xm.ToString());
            }
            return xm.ToString();
        }
        //nuevo guarda un campo secuencia para pescar carga
        public static Int64? TraceMove(Int64 sq,string usuario, string origen, string clase, string metodo, string resultado_ejecucion)
        {
            try
            {
                init();
                if (!_errsql) { return -2; }
                Int64 result = 0;
                string conn = db.CadenaBasica;
                conn = string.Format("{0};Enlist=\"false\"", conn);
                using (var conexion = new SqlConnection(conn))
                {
                    try
                    {
                        using (var comando = conexion.CreateCommand())
                        {
                            comando.CommandType = CommandType.StoredProcedure;
                            comando.CommandText = "[brbk].[insertLog]";
                            comando.Parameters.AddWithValue("@user", usuario);
                            comando.Parameters.AddWithValue("@source", origen);
                            comando.Parameters.AddWithValue("@method", metodo);
                            comando.Parameters.AddWithValue("@result", false);
                           // comando.Parameters.AddWithValue("@sqc", sq);
                            comando.Transaction = null;
                            //Objeto
                            comando.Parameters.AddWithValue("@object", clase);
                            //parametros

                            comando.Parameters.AddWithValue("@result_execution_message", resultado_ejecucion);
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

    }
}
