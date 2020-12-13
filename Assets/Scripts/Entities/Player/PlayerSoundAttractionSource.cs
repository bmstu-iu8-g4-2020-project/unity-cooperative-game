using Entities.Zombie;
using Gameplay;
using Mirror;
using UnityEngine;

namespace Entities.Player
{
    public class PlayerSoundAttractionSource : SoundAttractionSource
    {
        [SerializeField]
        private SphereCollider soundTrigger;

        [SerializeField]
        private NetworkIdentity identity;

        [field: SerializeField]
        public float RadiusForSprint { get; } = 10;

        [field: SerializeField]
        public float RadiusForWalk { get; } = 4;

        [field: SerializeField]
        public float RadiusForStealth { get; } = 2;

        public new float Radius
        {
            get => soundTrigger.radius;
            set => soundTrigger.radius = value;
        }

        private void Start()
        {
            soundTrigger.radius = Radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Zombie.Zombie zombie))
            {
                if (identity is null)
                    Debug.LogError($"{nameof(NetworkIdentity)} component not found on object {name}");

                zombie.CmdTrySetTarget(identity, (int) AttractionSourceType.Sound);
            }
        }
    }
}
