using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Gameplay
{
    internal enum ContainerType
    {
        Default,
        Kitchen,
        Police,
        Garage
    }

    public class Container : ItemContainer
    {
        private static Dictionary<ContainerType, ItemData[]> _lootByContainerType;

        [SerializeField]
        private ContainerType type;

        private void Start()
        {
        }

        public override void OnStartServer()
        {
            GenerateLoot();
        }

        private void GenerateLoot()
        {
        }
    }
}
