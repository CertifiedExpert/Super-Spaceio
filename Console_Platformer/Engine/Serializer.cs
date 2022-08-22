using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace Console_Platformer.Engine
{
    public static class Serializer
    {
        public static void ToFile<T>(T instance, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fs, instance);
            } 
        }

        public static T FromFile<T>(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(fs);
            }
        }
    }
}
