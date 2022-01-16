using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace System.NetPro
{
    /// <summary>
    /// Xml序列化与反序列化
    /// </summary>
    public class XmlHelper
    {
        private static readonly object LockSerialize = new object();

        /// <summary>
        /// 将自定义对象序列化为XML字符串
        /// </summary>
        /// <param name="myClass">自定义对象实体</param>
        /// <returns>序列化后的XML字符串</returns>
        public static string SerializeToXml(object myClass)
        {
            if (myClass != null)
            {
                XmlSerializer xs = new XmlSerializer(myClass.GetType());

                MemoryStream stream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Formatting.None;//缩进
                xs.Serialize(writer, myClass);

                stream.Position = 0;
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                    reader.Close();
                }
                writer.Close();
                return sb.ToString();
            }
            return string.Empty;
        }


        public static void XmlSerializer(object o, string path)
        {
            lock (LockSerialize)
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new InvalidOperationException());
                }
                var stream = new FileStream(path, FileMode.Create);
                XmlSerializer t = new XmlSerializer(o.GetType(), "");
                t.Serialize(stream, o);
                stream.Close();
            }
        }

        public static object XmlDeserialize(string path, Type type)
        {
            lock (LockSerialize)
            {
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                XmlSerializer t = new XmlSerializer(type, "");
                object o = t.Deserialize(stream);
                stream.Close();
                return o;
            }
        }

        public static T XmlReader<T>(string fileName)
        {
            if (!File.Exists(fileName)) return default(T);
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
                stream.Position = 0;
                return (T)xmlFormat.Deserialize(stream);
            }
        }

        public static void XmlWrite<T>(T t, string fileName)
        {
            using (Stream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                XmlSerializer xmlFormat = new XmlSerializer(typeof(T));
                xmlFormat.Serialize(stream, t);
            }
        }

        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {

            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(sr);
            }

        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
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
        public static string Serializer(Type type, object obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(type);
                xml.Serialize(stream, obj);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion
    }
}
