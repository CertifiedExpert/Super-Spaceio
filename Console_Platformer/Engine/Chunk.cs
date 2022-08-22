using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
//TODO: temp
using SpaceGame;

namespace Console_Platformer.Engine
{
    [DataContract]
    //[KnownType(typeof(GameObject))]

    class Chunk
    {
        [DataMember]
        public List<GameObject>[] gameObjectRenderLists = new List<GameObject>[Engine.spriteLevelCount];
        [DataMember]
        public List<GameObject> gameObjects = new List<GameObject>();

        public List<GameObject> gameObjectsToRemove = new List<GameObject>();
        public List<GameObject> gameObjectsToAdd = new List<GameObject>();
        public DateTime lastUnloaded = DateTime.MinValue;
        public Vec2i Index { get; set; }
        public Engine Engine { get; set; }
        public Chunk(Vec2i index, Engine engine)
        {
            Engine = engine;
            for (var i = 0; i < gameObjectRenderLists.Length; i++)
            {
                gameObjectRenderLists[i] = new List<GameObject>();
            }

            Index = index;
        }
    }
}
