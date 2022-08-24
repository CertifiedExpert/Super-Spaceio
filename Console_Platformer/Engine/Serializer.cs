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
    }
}
