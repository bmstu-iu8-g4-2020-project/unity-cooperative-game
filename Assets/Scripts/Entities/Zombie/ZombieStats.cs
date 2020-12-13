using Entities.Attributes;
using UnityEngine.Serialization;

namespace Entities.Player
{
    public class ZombieStats : EntityStats
    {
        [FormerlySerializedAs("speed")]
        public FloatAttribute roamingSpeed = new FloatAttribute(1f);

        public FloatAttribute chaseSpeed = new FloatAttribute(3f);
        public IntAttribute attack = new IntAttribute(1);
    }
}
