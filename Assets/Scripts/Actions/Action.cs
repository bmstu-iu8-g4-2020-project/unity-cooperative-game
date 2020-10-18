using Gameplay;
using Player;
using UnityEngine;

namespace Actions
{
    /// <summary>
    /// Interactable Action parent class. Actions that player can do with somethig
    /// </summary>
    public class Action : ScriptableObject
    {
        public string title;

        //When using an action on a Interactable in the scene
        public virtual void DoAction(PlayerCharacter character, Interactable interactable)
        {
        }

        //Todo mb make UIItemSlot interactable or selectable?
        // public virtual void DoAction(PlayerCharacter character, UIItemSlot slot)
        // {
        // }

        //Condition to check if the action is possible, override to add a condition
        public virtual bool CanDoAction(PlayerCharacter character, Interactable interactable)
        {
            return true; //No condition
        }
    }
}
