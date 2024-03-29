﻿using Gameplay;
using Player;

namespace Actions
{
    /// <summary>
    /// Automatic Action parent class: Any action performed automatically when the object on interact by key.
    /// Ex: opening window or door, climbing
    /// </summary>
    public class AAction : Action
    {
        //When using an action on interactable in the scene
        public override void DoAction(PlayerCharacter character, Interactable interactable)
        {
        }

        public virtual void StopAction()
        {
        }

        //Condition to check if the action is possible, override to add a condition
        public override bool CanDoAction(PlayerCharacter character, Interactable interactable) => true;
    }
}
