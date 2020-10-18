using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData", order = 3)]
    public class PlayerData : ScriptableObject
    {
        [field: SerializeField]
        public float WalkSpeed { get; private set; } = 10f;

        public float WalkWithActionSpeedMultiplier { get; private set; } = 0.7f;

        [field: SerializeField]
        public float SprintSpeedMultiplier { get; private set; } = 1.5f;

        [field: SerializeField]
        public float StealthSpeed { get; private set; } = 5f;


        [field: SerializeField]
        public float ClimbingDuration { get; private set; } = 2.0f;
    }
}
