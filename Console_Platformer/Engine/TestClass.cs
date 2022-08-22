using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Console_Platformer.Engine
{
    [DataContract]
    class TestClass
    {
        [DataMember]
        public Vec2i TestPos { get; set; }
        [DataMember]
        public bool TestBool { get; set; }
        [DataMember]
        public Collider[] TestColliderArr = new Collider[2];
        public TestClass(Vec2i testPos, bool testBool)
        {
            TestPos = testPos;
            TestBool = testBool;
        }
    }
}
