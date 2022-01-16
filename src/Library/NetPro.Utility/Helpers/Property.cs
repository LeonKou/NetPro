using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace System.NetPro
{
    [Serializable]
    [XmlRoot("Property")]
    public class Property : IXmlSerializable
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        public string this[string key]
        {
            get
            {
                if (_properties.ContainsKey(key))
                    return _properties[key];
                return null;
            }
            set
            {
                _properties[key] = value;
            }
        }

        public Dictionary<string, string> Properties
        {
            get
            {
                return _properties;
            }
        }

        #region IXmlSerializable 成员

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
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
                _properties.Add(key ?? throw new InvalidOperationException(), value);
            }
            reader.ReadEndElement();

        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (string key in _properties.Keys)
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
