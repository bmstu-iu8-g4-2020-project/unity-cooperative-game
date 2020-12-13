using Gameplay;
using UnityEngine;

namespace Actions
{
    //Todo Or use dict
    /// <summary>
    /// Interactable Action parent class. Actions that player can do with somethig
    /// </summary>
    public class Action : ScriptableObject
    {
        public string title;

        //When using an action on a Interactable in the scene
        public virtual void DoAction(Entities.Player.PlayerController character, Interactable interactable)
        {
        }

        //Todo mb make UIItemSlot interactable or selectable?
        // public virtual void DoAction(PlayerController character, UIItemSlot slot)
        // {
        // }

        //Condition to check if the action is possible, override to add a condition
        public virtual bool CanDoAction(Entities.Player.PlayerController character, Interactable interactable) => true;
    }
}
