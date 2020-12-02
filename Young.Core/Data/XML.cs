using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Young.Core.Data
{
    /// <summary>
    /// XML操作类
    /// </summary>
    public class XML
    {
        #region (public) xml序列化 _XMLSerialize
        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="obj">obj类</param>
        /// <returns>string字符串</returns>
        public static string _XMLSerialize(object obj)
        {
            XmlSerializer xs = new XmlSerializer(obj.GetType());
            StringBuilder strBuidler = new StringBuilder();
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.OmitXmlDeclaration = true;//去掉xml版本声明
            System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(strBuidler, setting);
            XmlSerializerNamespaces xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);
            xs.Serialize(xw, obj, xmlns);
            xw.Close();
            return strBuidler.ToString();
        }

        #endregion

        #region (public) xml序列化 _ConvertToString
        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="obj">obj类</param>
        /// <returns>string字符串</returns>
        public static string _ConvertToString(object objectToConvert)
        {
            string xml = null;
            if (objectToConvert == null)
                return xml;

            Type t = objectToConvert.GetType();

            XmlSerializer ser = new XmlSerializer(t);
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                ser.Serialize(writer, objectToConvert, ns);
                xml = writer.ToString();
                writer.Close();
            }
            return xml;
        }

        #endregion

        #region (public) xml反序列化 _XMLDeserialize
        /// <summary>
        /// xml反序列化
        /// </summary>
        /// <param name="s">字符串string</param>
        /// <param name="type">obj type</param>
        /// <returns>obj</returns>
        public static object _XMLDeserialize(string s, Type type)
        {


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(s);
            s = xmlDoc.DocumentElement.OuterXml;
            XmlSerializer xs = new XmlSerializer(type);
            Stream stream = new System.IO.MemoryStream(System.Text.ASCIIEncoding.UTF8.GetBytes(s));
            object obj = xs.Deserialize(stream);
            stream.Close();
            return obj;
        }



        #endregion

        #region (public) xml反序列化 _XML2DataSet

        /// <summary>
        /// xml to dataset
        /// </summary>
        /// <param name="xmlData">xml字符串</param>
        /// <returns>dataset</returns>
        public static DataSet _XML2DataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (Exception ex)
            {
                string strTest = ex.Message;
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        #endregion

        #region (public) XML反序列化 _ConvertFileToObject
        /// <summary>
        /// 读取文件转化为对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="objectType">对象类型</param>
        /// <returns>对象</returns>
        public static object _ConvertFileToObject(string path, Type objectType)
        {
            object convertedObject = null;

            if (path != null && path.Length > 0)
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer ser = new XmlSerializer(objectType);
                    convertedObject = ser.Deserialize(fs);
                    fs.Close();
                }
            }
            return convertedObject;
        }

        #endregion

        #region (public) xml 反序列化 _ConvertXmlToObject
        /// <summary>
        /// 将xml字符串转换为对应的实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="xmlDocument">xml字符串</param>
        /// <returns>实体类型对象</returns>
        public static T _ConvertXmlToObject<T>(string xml) where T : class, new()
        {
            if (string.IsNullOrEmpty(xml)) return new T();
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T resultObject;
            using (TextReader reader = new StringReader(xml))
            {
                resultObject = (T)serializer.Deserialize(reader);
            }
            return resultObject;
        }

        #endregion

        #region(public) 对象保存为xml _SaveAsXML
        /// <summary>
        ///  把对象序列化为XML 并保存为文件
        /// </summary>
        /// <param name="objectToConvert">对象</param>
        /// <param name="path">路径</param>
        public static void _SaveAsXML(object objectToConvert, string path)
        {
            if (objectToConvert != null)
            {
                Type t = objectToConvert.GetType();
                XmlSerializer ser = new XmlSerializer(t);
                using (StreamWriter writer = new StreamWriter(path))
                {
                    ser.Serialize(writer, objectToConvert);
                    writer.Close();
                }
            }
        }

        #endregion

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>( string xml, ref string strMessage ) where T :class, new()
        {
            try
            {
                using( StringReader sr = new StringReader(xml) )
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(sr);
                }
            }
            catch( Exception ex )
            {
                strMessage = ex.Message;
                return null;
            }

        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize( Type type, Stream stream )
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer<T>( T obj, ref string strMessage ) where T :class, new()
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(typeof(T));
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);

                Stream.Position = 0;
                StreamReader sr = new StreamReader(Stream);
                string str = sr.ReadToEnd();

                sr.Dispose();
                Stream.Dispose();

                return str;
            }
            catch( InvalidOperationException ex )
            {
                strMessage = ex.Message;

                return string.Empty;
            }
        }

        #endregion

    }
}
