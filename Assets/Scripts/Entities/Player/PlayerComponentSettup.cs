using Mirror;
using UnityEngine;

namespace Entities.Player
{
    public class PlayerComponentSettup : NetworkBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] scriptsOnlyForLocalPlayer;

        [SerializeField]
        private GameObject[] childGameObjectsForLocalPlayerOnly;

        #region Initialize

        private void Start()
        {
            foreach (var script in GetComponents<MonoBehaviour>())
                if (!(script is NetworkBehaviour))//disable all except NetworkBehaviour
                    script.enabled = isLocalPlayer;

            foreach (var gObject in childGameObjectsForLocalPlayerOnly)
                if (gObject != null)
                    gObject.SetActive(isLocalPlayer);
        }

        #endregion
    }
}
