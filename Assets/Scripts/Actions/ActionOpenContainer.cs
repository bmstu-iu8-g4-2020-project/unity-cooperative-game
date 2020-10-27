using Gameplay;
using Player;
using UI;
using UnityEngine;

namespace Actions
{
    [CreateAssetMenu(fileName = "OpenContainer", menuName = "Data/Actions/OpenContainer", order = 0)]
    public class ActionOpenContainer : AAction
    {
        public override void DoAction(PlayerCharacter character, Interactable interactable)
        {
            UIController.Instance.ContainerUI.OpenContainer(interactable.GetComponent<Gameplay.ItemContainer>());
            UIController.Instance.InventoryUI.OpenContainer(character.GetComponent<Gameplay.ItemContainer>());

            //trigger animation and etc
        }

        public override void StopInteraction()
        {
            DelayedOperationsManager.Instance.DequeueAllOperations();
            UIController.Instance.ContainerUI.CloseContainer();
            UIController.Instance.InventoryUI.CloseContainer();
        }
    }
}
