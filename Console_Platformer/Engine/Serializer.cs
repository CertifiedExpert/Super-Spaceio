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
        // List of types recognised by the Serializer.
        public List<Type> knownTypes = new List<Type>() {typeof(GameObject) }; 

        // Saves an instance of T to the specified file.
        public void ToFile<T>(T instance, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                serializer.WriteObject(fs, instance);
            } 
        }
        // Returns an instance of T from a specified file.
        public T FromFile<T>(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                return (T)serializer.ReadObject(fs);
            }
        }

        // Returs a string of xml data of the specified instance of T.
        public string ToXmlString<T>(T instance)
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

        // Returns an instance of T from the specified string of xml data.
        public T FromXmlString<T>(string xml, T instance)
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
