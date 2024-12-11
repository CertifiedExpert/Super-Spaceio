using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
