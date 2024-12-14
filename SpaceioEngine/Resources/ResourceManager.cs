using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ConsoleEngine
{
    public class ResourceManager
    {
        private Engine Engine { get; set; }
        private ResIDManager ResIDManager { get; set; }

        public string resFolderPath; 

        public ResourceManager(Engine engine, ResIDManager resIDManager)
        {
            Engine = engine;
            ResIDManager = resIDManager;
            Bitmaps = new ReadOnlyDictionary<ResID, Bitmap>(bitmaps);
        }

        internal ResourceManager(Engine engine, ResourceManagerSaveData saveData)
        {
            Engine = engine;
            Bitmaps = new ReadOnlyDictionary<ResID, Bitmap>(bitmaps);
            resFolderPath = $"{engine.pathRootFolder}\\Resources";

            ResIDManager = new ResIDManager(saveData.ResIDManager);
        }

        private Dictionary<string, ResID> namesAndResIDs = new Dictionary<string, ResID>(); 
        private Dictionary<ResID, Bitmap> bitmaps = new Dictionary<ResID, Bitmap>();
        public ReadOnlyDictionary<ResID, Bitmap> Bitmaps { get; }

        internal void Init()
        {
            resFolderPath = $"{Engine.pathRootFolder}\\Resources";
        }

        internal void LoadAllBitmapsFromFolder(string path)
        {
            try
            {
                var filesPaths = Directory.GetFiles(path);

                foreach (var p in filesPaths)
                {
                    var tuple = LoadBitmapFromPath(p);

                    if (bitmaps.ContainsKey(tuple.Item1)) bitmaps[tuple.Item1] = tuple.Item2;
                    else
                    {
                        var resID = AddBitmap(tuple.Item2);
                        namesAndResIDs.Add(Path.GetFileNameWithoutExtension(p), resID);
                    }
                }
            }
            catch { }
        }

        // Returns ResIDManager.InvalidResID if no resourche with this name exists
        public ResID GetResID(string name)
        {
            if (namesAndResIDs.ContainsKey(name)) return namesAndResIDs[name];
            else return ResIDManager.InvalidResID;
        }

        public ResID AddBitmap(Bitmap bitmap)
        {
            var resID = ResIDManager.GenerateResID();
            bitmaps.Add(resID, bitmap);
            return resID;
        }

        public void RemoveBitmap(ResID resId)
        {
            bitmaps.Remove(resId);
            ResIDManager.RetireResID(resId);
        }

        private Tuple<ResID, Bitmap> LoadBitmapFromPath(string p)
        {
            throw new NotImplementedException(); //TODO: write this
        }

        internal ResourceManagerSaveData GetSaveData()
        {
            var sd = new ResourceManagerSaveData();
            sd.ResIDManager = ResIDManager.GetSaveData();
            return sd;
        }
        


        public static Bitmap fighter1Up;
        public static Bitmap fighter1Down;
        public static Bitmap fighter1Right;
        public static Bitmap fighter1Left;

        public static Bitmap fighter1UpLeft;
        public static Bitmap fighter1UpRight;
        public static Bitmap fighter1DownLeft;
        public static Bitmap fighter1DownRight;

        public static Bitmap enemyDefault;
        public static Bitmap enemyDefault2;

        public static Bitmap asteroid;
        public static void Initialize()
        {
            #region FighterSprites

            var fighter1UpBitmap = new char[9, 9]
            {
                {' ',' ', ' ', ' ', '%',' ', ' ', ' ', ' '},
                {' ',' ', ' ', ' ', '%',' ', ' ', ' ', ' '},
                {' ',' ', ' ', '%', '.','%', ' ', ' ', ' '},
                {' ',' ', ' ', '%', '.','%', ' ', ' ', ' '},
                {' ',' ', '%', '.', 'O','.', '%', ' ', ' '},
                {'!',' ', '%', '.', 'O','.', '%', ' ', '!'},
                {'|','%', '.', '.', '.','.', '.', '%', '|'},
                {'@','%', '%', '%', '%','%', '%', '%', '@'},
                {' ',' ', '\\', '#', ' ','#', '/', ' ', ' '},
            };
            fighter1Up = new Bitmap(PrepareData(fighter1UpBitmap));

            var fighter1DownBitmap = new char[9, 9]
            {
                {' ',' ', '/', '#', ' ','#', '\\', ' ', ' '},
                {'@','%', '%', '%', '%','%', '%', '%', '@'},
                {'|','%', '.', '.', '.','.', '.', '%', '|'},
                {'!',' ', '%', '.', 'O','.', '%', ' ', '!'},
                {' ',' ', '%', '.', 'O','.', '%', ' ', ' '},
                {' ',' ', ' ', '%', '.','%', ' ', ' ', ' '},
                {' ',' ', ' ', '%', '.','%', ' ', ' ', ' '},
                {' ',' ', ' ', ' ', '%',' ', ' ', ' ', ' '},
                {' ',' ', ' ', ' ', '%',' ', ' ', ' ', ' '},
            };
            fighter1Down = new Bitmap(PrepareData(fighter1DownBitmap));

            var fighter1RightBitmap = new char[9, 9]
            {
                {' ','@', '―', '-', ' ',' ', ' ', ' ', ' '},
                {' ','%', '%', ' ', ' ',' ', ' ', ' ', ' '},
                {'/','%', '.', '%', '%',' ', ' ', ' ', ' '},
                {'#','%', '.', '.', '.','%', '%', ' ', ' '},
                {' ','%', '.', 'O', 'O','.', '.', '%', '%'},
                {'#','%', '.', '.', '.','%', '%', ' ', ' '},
                {'\\','%', '.', '%', '%',' ', ' ', ' ', ' '},
                {' ','%', '%', ' ', ' ',' ', ' ', ' ', ' '},
                {' ','@', '―', '-', ' ',' ', ' ', ' ', ' '},
            };
            fighter1Right = new Bitmap(PrepareData(fighter1RightBitmap));


            var fighter1LeftBitmap = new char[9, 9]
            {
                {' ',' ', ' ', ' ', ' ','-', '―', '@', ' '},
                {' ',' ', ' ', ' ', ' ',' ', '%', '%', ' '},
                {' ',' ', ' ', ' ', '%','%', '.', '%', '\\'},
                {' ',' ', '%', '%', '.','.', '.', '%', '#'},
                {'%','%', '.', '.', 'O','O', '.', '%', ' '},
                {' ',' ', '%', '%', '.','.', '.', '%', '#'},
                {' ',' ', ' ', ' ', '%','%', '.', '%', '/'},
                {' ',' ', ' ', ' ', ' ',' ', '%', '%', ' '},
                {' ',' ', ' ', ' ', ' ','-', '―', '@', ' '},
            };
            fighter1Left = new Bitmap(PrepareData(fighter1LeftBitmap));


            var fighter1UpLeftBitmap = new char[9, 9]
            {
                {'%','%', '%', ' ', ' ',' ', '\\', ' ', ' '},
                {'%','.', '.', '%', '%','%', ' ', '\\', ' '},
                {'%','.', '.', '.', '.','.', '%', '%', '@'},
                {' ','%', '.', 'O', '.','.', '.', '%', ' '},
                {' ','%', '.', '.', 'O','.', '%', '|', ' '},
                {' ','%', '.', '.', '.','%', ' ', '#', ' '},
                {'\\',' ', '%', '.', '%','.', ' ', ' ', ' '},
                {' ','\\', '%', '%', '―','#', ' ', ' ', ' '},
                {' ',' ', '@', ' ', ' ',' ', ' ', ' ', ' '}
            };
            fighter1UpLeft = new Bitmap(PrepareData(fighter1UpLeftBitmap));

            var fighter1UpRightBitmap = new char[9, 9]
            {
                {' ',' ', '/', ' ', ' ',' ', '%', '%', '%'},
                {' ','/', '.', '%', '%','%', '.', '.', '%'},
                {'@','%', '%', '.', '.','.', '.', '.', '%'},
                {' ','%', '.', '.', '.','O', '.', '%', ' '},
                {' ','|', '%', '.', 'O','.', '.', '%', ' '},
                {' ','#', ' ', '%', '.','.', '.', '%', ' '},
                {' ',' ', ' ', ' ', '%','.', '%', ' ', '/'},
                {' ',' ', ' ', '#', '―','%', '%', '/', ' '},
                {' ',' ', ' ', ' ', ' ',' ', '@', ' ', ' '}
            };
            fighter1UpRight = new Bitmap(PrepareData(fighter1UpRightBitmap));

            var fighter1DownLeftBitmap = new char[9, 9]
            {
                {' ',' ', '@', ' ', ' ',' ', ' ', ' ', ' '},
                {' ','/', '%', '%', '―','#', ' ', ' ', ' '},
                {'/',' ', '%', '.', '%',' ', ' ', ' ', ' '},
                {' ','%', '.', '.', '.','%', ' ', '#', ' '},
                {' ','%', '.', '.', 'O','.', '%', '|', ' '},
                {' ','%', '.', 'O', '.','.', '.', '%', ' '},
                {'%','.', '.', '.', '.','.', '%', '%', '@'},
                {'%','.', '.', '%', '%','%', ' ', '/', ' '},
                {'%','%', '%', ' ', ' ',' ', '/', ' ', ' '},
            };
            fighter1DownLeft = new Bitmap(PrepareData(fighter1DownLeftBitmap));

            var fighter1DownRightBitmap = new char[9, 9]
            {
                {' ',' ', ' ', ' ', ' ',' ', '@', ' ', ' '},
                {' ',' ', ' ', '#', '―','%', '%', '\\', ' '},
                {' ',' ', ' ', ' ', '%','.', '%', ' ', '\\'},
                {' ','#', ' ', '%', '.','.', '.', '%', ' '},
                {' ','|', '%', '.', 'O','.', '.', '%', ' '},
                {' ','%', '.', '.', '.','O', '.', '%', ' '},
                {'@','%', '%', '.', '.','.', '.', '.', '%'},
                {' ','\\', ' ', '%', '%','%', '.', '.', '%'},
                {' ',' ', '\\', ' ', ' ',' ', '%', '%', '%'},
            };
            fighter1DownRight = new Bitmap(PrepareData(fighter1DownRightBitmap));
            #endregion

            #region EnemyDefaults

            var enemyDefaultBitmap = new char[4, 3]
            {
                {' ', '#', ' '},
                {'#', '+', '#'},
                {'#', '+', '#'},
                {'#', '+', '#'},
            };
            enemyDefault = new Bitmap(PrepareData(enemyDefaultBitmap));

            var enemyDefault2Bitmap = new char[4, 3]
            {
                {' ', '#', ' '},
                {'#', ' ', '#'},
                {'#', ' ', '#'},
                {'#', ' ', '#'},
            };
            enemyDefault2 = new Bitmap(PrepareData(enemyDefault2Bitmap));
            #endregion

            var asteroidBitmap = new char[18, 18]
            {
                {' ', ' ', ' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
                {' ', ' ', ' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', ' ', ' ', ' ', ' ', ' ', ' ', ' '},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', ' ', ' ', ' '},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', ' ', ' ', ' '},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', ' ', ' ', ' '},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', ' ', ' ', ' '},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', ' ', ' ', ' '},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {'⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {'⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {'⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {'⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {'⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {' ', ' ', ' ', ' ', ' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {' ', ' ', ' ', ' ', ' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
                {' ', ' ', ' ', ' ', ' ', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛', '⬛'},
            };

            asteroid = new Bitmap(PrepareData(asteroidBitmap));
        }


        // Rotates the data 90 degrees to the left to a allow a cleaner representation of sprites in char[]
        public static char[,] PrepareData(char[,] prior)
        {
            var priorX = prior.GetLength(0);
            var priorY = prior.GetLength(1);

            var data = new char[priorY, priorX];

            for (var x = 0; x < data.GetLength(0); x++)
            {
                for (var y = 0; y < data.GetLength(1); y++)
                {
                    data[x, y] = prior[priorX - y - 1, x];
                }
            }

            return data;
        }
    }
}
