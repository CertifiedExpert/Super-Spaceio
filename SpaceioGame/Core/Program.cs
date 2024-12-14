using ConsoleEngine;

namespace Spaceio
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var game = new Game();

            var settings = new Settings(10, ' ', "  " , 1000, 196, 99, 1000);
            var chunkManager = new ChunkManager(game);
            var uIDManager = new UIDManager();
            var gameObjectManager = new GameObjectManager(game, uIDManager);
            var camera = new Camera(new Vec2i(0, 0), new Vec2i(settings.CameraSizeX, settings.CameraSizeY));
            var resIDManager = new ResIDManager();
            var resourceManager = new ResourceManager(game, resIDManager);
            var saveFileManager = new SaveFileManager(game);
            var renderer = new Renderer(game);
            var inputManager = new InputManager();

            game.NewSave("test", settings, chunkManager, gameObjectManager, camera, resourceManager,
                saveFileManager, renderer, inputManager);

            game.Run();

            Console.ReadLine();
        }
    }
}