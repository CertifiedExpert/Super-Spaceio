using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Console_Platformer.Engine
{
    class Serializer
    {
        //public EngineDataContractResolver engineDataContractResolver { get; set; }
        public List<Type> knownTypes = new List<Type>() {typeof(GameObject) };
        public void ToFile<T>(T instance, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                serializer.WriteObject(fs, instance);
            } 
        }
        public T FromFile<T>(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                return (T)serializer.ReadObject(fs);
            }
        }

        
        public string ToString<T>(T instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(instance.GetType(), knownTypes);
                serializer.WriteObject(memoryStream, instance);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }
        public T FromString<T>(string xml, T instance)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T), knownTypes);
                return (T)deserializer.ReadObject(stream);
            }
        }
    }
}
