using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Entities.Player
{
    public class PlayerComponentSetup : NetworkBehaviour
    {
        [SerializeField]
        private List<MonoBehaviour> monoBehavioursForServer;

        [SerializeField]
        private GameObject[] childGameObjectsForLocalPlayerOnly;

        #region Initialize

        private void Start()
        {
            foreach (var script in GetComponents<MonoBehaviour>())
                if (!(script is NetworkBehaviour)) //disable all except NetworkBehaviour
                    script.enabled = isLocalPlayer;

            foreach (var gObject in childGameObjectsForLocalPlayerOnly)
                if (gObject != null)
                    gObject.SetActive(isLocalPlayer);
        }

        #endregion
    }
}
