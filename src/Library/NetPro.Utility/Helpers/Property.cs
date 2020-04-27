using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NetPro.Utility.Helpers
{
    [Serializable]
    [XmlRoot("Property")]
    public class Property : IXmlSerializable
    {
        private Dictionary<string, string> properties = new Dictionary<string, string>();
        public string this[string key]
        {
            get
            {
                if (properties.ContainsKey(key))
                    return properties[key];
                return null;
            }
            set
            {
                properties[key] = value;
            }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                return properties;
            }
        }

        #region IXmlSerializable 成员

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(string));

            if (reader.IsEmptyElement || !reader.Read())
            {
                return;
            }
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Comment)
                {
                    reader.ReadEndElement();
                    reader.MoveToContent();
                    continue;
                }
                string key = reader.GetAttribute("key");

                reader.ReadStartElement("item");

                string value = reader.ReadContentAsString();

                reader.ReadEndElement();
                reader.MoveToContent();
                properties.Add(key, value);
            }
            reader.ReadEndElement();

        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (string key in properties.Keys)
            {
                writer.WriteStartElement("item");

                writer.WriteAttributeString("key", key);

                writer.WriteCData(this[key]);

                writer.WriteEndElement();
            }

        }

        #endregion
    }
}
