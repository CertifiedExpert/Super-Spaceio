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
        [DataMember] public UID UID;
        [DataMember] public Vec2i Position;
        [DataMember] public int SpriteLevel;
        [DataMember] public List<SpriteSaveData> Sprites;
        [DataMember] public List<ColliderSaveData> Colliders;
    }
}
