using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace SqlConexion
{
    public class billion_log
    {


        private static string SQLControl(SqlException s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (SqlError ei in s.Errors)
            {
                sb.AppendLine(string.Format("{0}, {1}, {2}, {3}", ei.LineNumber, ei.Message, ei.Procedure, ei.Server));
            }
            return sb.ToString();
        }


        public static Int64? LogEvent<T>(string _user, string _source, string _method, bool _result, string _object = null, Dictionary<string, object> _parameter_input = null, string _result_execution_message = null, T _exception = null) where T : System.Exception
        {
            try
            {
                string conn = System.Configuration.ConfigurationManager.ConnectionStrings["BILLION"]?.ConnectionString;
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
                            comando.CommandText = "sp_Bil_insert_log";
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
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
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
