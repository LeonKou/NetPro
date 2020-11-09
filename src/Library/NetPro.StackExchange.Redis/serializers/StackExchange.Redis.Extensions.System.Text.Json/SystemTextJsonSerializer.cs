using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;

using StackExchange.Redis.Extensions.Core;

namespace StackExchange.Redis.Extensions.System.Text.Json
{
    /// <summary>
    /// System.Text.Json implementation of <see cref="ISerializer"/>
    /// </summary>
    public class SystemTextJsonSerializer : ISerializer
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public T Deserialize<T>(byte[] serializedObject)
        {
            if (typeof(T) == typeof(string) || typeof(T).IsValueType)
            {
                var jsonString = encoding.GetString(serializedObject);
                jsonString = jsonString.TrimStart('\"').TrimEnd('\"');
                return (T)Convert.ChangeType(jsonString, typeof(T));
            }
            return JsonSerializer.Deserialize<T>(serializedObject, SerializationOptions.Flexible);
        }

        public byte[] Serialize(object item)
        {
            return JsonSerializer.SerializeToUtf8Bytes(item, SerializationOptions.Flexible);
        }
    }
}
