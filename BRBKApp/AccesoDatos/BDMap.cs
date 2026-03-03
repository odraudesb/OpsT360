using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace AccesoDatos
{
    public class BDMap
    {
        public static string Serializar<T>(T obj, out string OnError) where T:class
        {
            OnError = string.Empty;
            string xmlStr = String.Empty;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            settings.NewLineChars = String.Empty;
            settings.NewLineHandling = NewLineHandling.None;
            //omita los caracteres especiales
            settings.CheckCharacters = false;
            try
            {
                using (StringWriter stringWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                    {
                        XmlSerializer serializer = new XmlSerializer(obj.GetType());
                        serializer.Serialize(xmlWriter, obj);
                        xmlStr = stringWriter.ToString();
                        xmlWriter.Close();
                    }
                    return xmlStr;
                }
            }
            catch (Exception ex)
            {
                OnError = ex.Message;
                return null;
            }
        }
        public static T Deserializar<T>(string data, out string OnError) where T : class
        {
            try
            {
                XmlSerializer xmlSer = new XmlSerializer(typeof(T));
                StringReader reader = new StringReader(data);
                OnError = string.Empty;
                return (T)(xmlSer.Deserialize(reader));
            }
            catch (Exception ex)
            {
                OnError = ex.Message;
                return null;
            }

        }
        public static T ReaderAObjeto<T>(IDataReader dr) where T:class,new()
        {
            var newObject = Activator.CreateInstance<T>();
            //Accesor
            var objectMemberAccessor = TypeAccessor.Create(newObject.GetType());
            var propertiesHashSet =
                    objectMemberAccessor
                    .GetMembers()
                    .Select(mp => mp.Name)
                    .ToList();
            //properties
            if (!dr.IsClosed)
            {
                var prun = new Dictionary<string, object>();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (propertiesHashSet.Contains(dr.GetName(i)))
                    {
                        if (prun.ContainsKey(dr.GetName(i)))
                        {
                            prun.Add(dr.GetName(i), !dr.IsDBNull(i)? dr.GetValue(i):"Nulo");
                        }
                        if (!dr.IsDBNull(i))
                        {
                            var t = dr.GetName(i);
                            var j = dr.GetValue(i);
                            try
                            {
                           
                                objectMemberAccessor[newObject, dr.GetName(i)] = dr.GetValue(i);
                               

                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                string tt = t;
                                var jj = j;
                                BDTraza.LogEvent<Exception>("CodigoFuente", nameof(ReaderAObjeto), "Conversion", false, nameof(BDMap), prun, "Error", ex);
#endif
                                continue;
                            }
                        }
                    }
                }
                //return}
           }
                return newObject;
        }

        public static Dictionary<string, string> ObtenerPropiedades<T>(T objeto) where T:class
        {
            try
            {
                var newObject = Activator.CreateInstance<T>();
                var objectMemberAccessor = TypeAccessor.Create(newObject.GetType());
                return objectMemberAccessor?.GetMembers().ToDictionary(mc => mc.Name, mc => mc.Type.Name);
            }
            catch
            {
                return null;
            }
        }

        public static Dictionary<string, Tuple<string, string>> ObtenerParametrosProc(string con, string pc)
        {
            var l = new Dictionary<string, Tuple<string, string>>();
            SqlConnection conn = new SqlConnection(con);
            SqlCommand cmd = new SqlCommand(pc, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            conn.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            foreach (SqlParameter p in cmd.Parameters)
            {
                l.Add(p.ParameterName, Tuple.Create(p.SqlDbType.ToString(), ObtenerTipoCLR(p.SqlDbType)?.Name));
            }
            return l;
        }



        private static Dictionary<Type, SqlDbType> typeReference;

        private static void CargaMapeo()
        {
            typeReference = new Dictionary<Type, SqlDbType>();
            typeReference.Add(typeof(string), SqlDbType.NVarChar);
            typeReference.Add(typeof(Guid), SqlDbType.UniqueIdentifier);
            typeReference.Add(typeof(long), SqlDbType.BigInt);
            typeReference.Add(typeof(byte[]), SqlDbType.Binary);
            typeReference.Add(typeof(bool), SqlDbType.Bit);
            typeReference.Add(typeof(DateTime), SqlDbType.DateTime);
            typeReference.Add(typeof(decimal), SqlDbType.Decimal);
            typeReference.Add(typeof(double), SqlDbType.Float);
            typeReference.Add(typeof(int), SqlDbType.Int);
            typeReference.Add(typeof(float), SqlDbType.Real);
            typeReference.Add(typeof(short), SqlDbType.SmallInt);
            typeReference.Add(typeof(byte), SqlDbType.TinyInt);
            typeReference.Add(typeof(object), SqlDbType.Udt);
            typeReference.Add(typeof(DataTable), SqlDbType.Structured);
            typeReference.Add(typeof(DateTimeOffset), SqlDbType.DateTimeOffset);

        }


        public static Type ObtenerTipoCLR(SqlDbType sqlDataType)
        {
            switch (sqlDataType)
            {
                case SqlDbType.BigInt:
                    return typeof(long?);

                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);

                case SqlDbType.Bit:
                    return typeof(bool?);

                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);

                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime?);

                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal?);

                case SqlDbType.Float:
                    return typeof(double?);

                case SqlDbType.Int:
                    return typeof(int?);

                case SqlDbType.Real:
                    return typeof(float?);

                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid?);

                case SqlDbType.SmallInt:
                    return typeof(short?);

                case SqlDbType.TinyInt:
                    return typeof(byte?);

                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);

                case SqlDbType.Structured:
                    return typeof(DataTable);

                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);

                default:
                    return typeof(DBNull);
            }
        }


    }

}
