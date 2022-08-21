using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    class Chunk
    {
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<GameObject> gameObjectsToRemove = new List<GameObject>();
        public List<GameObject> gameObjectsToAdd = new List<GameObject>();
        public List<GameObject>[] gameObjectRenderLists = new List<GameObject>[Engine.spriteLevelCount];
        public DateTime lastUnloaded = DateTime.MinValue;

        private bool _isLoaded;
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set 
            {
                if (_isLoaded == true && value == false) lastUnloaded = DateTime.Now;
                else if (_isLoaded == false && value == true) OnChunkLoaded();
                _isLoaded = value;
            }
        }

        public Chunk()
        {
            for (var i = 0; i < gameObjectRenderLists.Length; i++)
            {
                gameObjectRenderLists[i] = new List<GameObject>();
            }
            IsLoaded = false;
        }
        private void OnChunkLoaded()
        {
            //TODO: write some logic here
        }
    }
}
