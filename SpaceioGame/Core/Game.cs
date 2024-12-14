using System.Runtime.Serialization;
using ConsoleEngine;

namespace Spaceio
{
    [DataContract(IsReference = true)]
    sealed class Game : Engine
    {
        // Settings
        public int millisecondsPerPlayerMove = 50;
        public const int chunkLoadRadius = 3;

        public int millisecondsSinceLastPlayerMove = 0;
        public bool playerMovedInThisFrame = false;
        protected override void OnLoad()
        {
            Serializer.knownTypes.Add(typeof(BaseObject));
            Serializer.knownTypes.Add(typeof(Ship));
        }

        protected override void Update()
        {
            if (InputManager.Escape.IsPressed) CloseEngine();

            UpdateCamera();

            CloseEngine();
        }

        private void UpdateCamera()
        {
            if (InputManager.ArrowUp.IsPressed) Camera.Move(0, Camera.Position.Y + 3);
            if (InputManager.ArrowDown.IsPressed) Camera.Move(0, Camera.Position.Y - 3);
            if (InputManager.ArrowLeft.IsPressed) Camera.Move(Camera.Position.X - 3, 0);
            if (InputManager.ArrowRight.IsPressed) Camera.Move(Camera.Position.X + 3, 0);
        }
        public UID AddBaseObject(BaseObject baseObject)
        {
            return AddGameObject(baseObject);
        }
        public void RemoveBaseObject(UID baseObjectUID)
        {
            RemoveGameObject(baseObjectUID);
        }

        protected override void OnSave()
        {
            
        }
    }
}
