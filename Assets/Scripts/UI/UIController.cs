using Player;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private GameObject uiInventory;

        private PlayerCharacter _character;

        private void Start()
        {
            _character = GameManager.Instance.GetLocalPlayer().GetComponent<PlayerCharacter>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                uiInventory.SetActive(!uiInventory.activeSelf);
            }
        }
    }
}
