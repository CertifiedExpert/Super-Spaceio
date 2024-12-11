using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    [DataContract]
    public class GameObjectSaveData
    {
        [DataMember] internal UID UID;
        [DataMember] internal Vec2i Position;
        [DataMember] internal int SpriteLevel;
        [DataMember] internal List<SpriteSaveData> Sprites;
        [DataMember] internal List<ColliderSaveData> Colliders;
    }
}
