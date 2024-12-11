using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public struct Shader
    {
        public delegate char GetPixel(int x, int y, Bitmap bitmap, object[] args);
        public GetPixel ShaderMethod { get; set; }
        public object[] Args { get; set; }

        public Shader(GetPixel shaderMethod, object[] args)
        {
            ShaderMethod = shaderMethod;
            Args = args;
        }

        internal Shader(ShaderSaveData saveData)
        {
            var targetType = Type.GetType(saveData.TargetType);
            var method = targetType.GetMethod(saveData.MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            ShaderMethod = (GetPixel)Delegate.CreateDelegate(typeof(GetPixel), method);

            Args = saveData.Args;
        }

        internal ShaderSaveData GetSaveData()
        {
            var sd = new ShaderSaveData();
            sd.MethodName = ShaderMethod.Method.Name;
            sd.TargetType = ShaderMethod.Method.DeclaringType.AssemblyQualifiedName;
            sd.Args = Args;
            return sd;
        }
    }
}
