using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WorldBeachesCMS.Helpers
{
    public class DataIO
    {
        public void SerializeObject<T>(T serializableObject, string filePath)
        {
            if (serializableObject == null) { return; }

            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, serializableObject);
            }
        }

        public T DeserializeObject<T>(string filePath)
        {
            if(!File.Exists(filePath)) { return default(T); }

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (T)serializer.Deserialize(reader);
            }
            

        }

    }
}
