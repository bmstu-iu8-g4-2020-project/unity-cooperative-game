using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [field: SerializeField]
        public UIContainerPanel InventoryUI { get; private set; }

        [field: SerializeField]
        public UIContainerPanel ContainerUI { get; private set; }
        
        [field: SerializeField]
        public Image OperationBarUI { get; private set; }

        [field: SerializeField]
        public UIPerTickAttributes PerTickAttributesBarsUI { get; private set; }

        #region singleton

        public static UIController Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.LogWarning($"More than one instance of a script in a scene. {gameObject.name}");
            }
        }

        #endregion

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) //TODO move to PlayerControl
            {
                if (InventoryUI.IsOpen)
                {
                    InventoryUI.CloseContainer();
                }
                else
                {
                    InventoryUI.OpenContainer(Entities.Player.PlayerController.LocalPlayer.Inventory);
                }
            }
        }
    }
}
