using Entities;
using Entities.Player;
using Gameplay;
using UI;
using UnityEngine;

namespace Actions
{
    [CreateAssetMenu(fileName = "OpenContainer", menuName = "Data/Actions/OpenContainer", order = 0)]
    public class ActionOpenContainer : AAction
    {
        public override void DoAction(PlayerController character, Interactable interactable)
        {
            UIController.Instance.ContainerUI.OpenContainer(interactable.GetComponent<ItemContainer>());
            UIController.Instance.InventoryUI.OpenContainer(character.Inventory);

            //trigger animation and etc
        }

        public override void StopAction()
        {
            DelayedOperationsManager.Instance.DequeueAllOperations();
            UIController.Instance.ContainerUI.CloseContainer();
            UIController.Instance.InventoryUI.CloseContainer();
        }
    }
}
