using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;

namespace ConsoleEngine
{
    public static class Serializer
    {
        // List of types recognized by the Serializer.
        public static List<Type> knownTypes = new List<Type>() {typeof(GameObject) };
        public static void AddKnownType(Type type) => knownTypes.Add(type);

        public static byte[] ToXmlBytes<T>(T instance)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(instance.GetType(), knownTypes);
                serializer.WriteObject(stream, instance);
                return stream.ToArray();
            }
        }
        public static T FromXmlBytes<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                return (T)serializer.ReadObject(stream);
            }
        }

        // Saves an instance of T to the specified file.
        public static void ToFile<T>(T instance, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                serializer.WriteObject(fs, instance);
            } 
        }
        // Returns an instance of T from a specified file.
        public static T FromFile<T>(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var serializer = new DataContractSerializer(typeof(T), knownTypes);
                return (T)serializer.ReadObject(fs);
            }
        }

        // Returns a string of xml data of the specified instance of T.
        public static string ToXmlString<T>(T instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8))
            {
                DataContractSerializer serializer = new DataContractSerializer(instance.GetType(), knownTypes);
                serializer.WriteObject(memoryStream, instance);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        // Returns an instance of T from the specified string of xml data.
        public static T FromXmlString<T>(string xml)
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
