using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Console_Platformer.Engine
{
    [DataContract(IsReference = true)]
    class Chunk
    {
        [DataMember]
        public List<GameObject>[] gameObjectRenderLists;
        [DataMember]
        public List<GameObject> gameObjects = new List<GameObject>();

        public List<GameObject> gameObjectsToRemove = new List<GameObject>();
        public List<GameObject> gameObjectsToAdd = new List<GameObject>();
        [DataMember]
        public DateTime lastUnloaded = DateTime.MinValue; //TODO: update this every time is unloaded
        public Vec2i Index { get; set; }
        public Engine Engine { get; set; }
        public Chunk(Vec2i index, Engine engine)
        {
            Engine = engine;

            gameObjectRenderLists = new List<GameObject>[Engine.spriteLevelCount];
            for (var i = 0; i < gameObjectRenderLists.Length; i++)
            {
                gameObjectRenderLists[i] = new List<GameObject>();
            }

            Index = index;
        }

        public void CompleteDataAfterSerialization(Engine engine, Vec2i index)
        {
            Engine = engine;
            Index = index;
            gameObjectsToAdd = new List<GameObject>();
            gameObjectsToRemove = new List<GameObject>();

            foreach (var gameObject in gameObjects)
            {
                gameObject.CompleteDataAfterSerialization(engine, index);
            }
        }

        public void InsertGameObject(GameObject gameObject)
        {
            gameObjectsToAdd.Add(gameObject);
            gameObjectRenderLists[gameObject.SpriteLevel].Add(gameObject);
        }
        public void UnInsertGameObject(GameObject gameObject)
        {
            gameObjectsToRemove.Add(gameObject);
            gameObjectRenderLists[gameObject.SpriteLevel].Remove(gameObject);
        }
    }
}
