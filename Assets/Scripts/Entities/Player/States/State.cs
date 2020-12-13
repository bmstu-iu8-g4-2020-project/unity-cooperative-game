using UnityEngine;
using UnityEngine.UI;

namespace Entities.Player.States
{
    public abstract class State
    {
        protected readonly PlayerController Character;
        protected readonly StateMachine StateMachine;

        private Text _fpsText;

        protected State(PlayerController character, StateMachine stateMachine)
        {
            Character = character;
            StateMachine = stateMachine;
        }

        public virtual void OnEnter()
        {
            _fpsText = GameManager.Instance.FPSText;
            GameManager.Instance.StateText.text = GetType().Name;
        }

        public virtual void Tick()
        {
            _fpsText.text = ((int) (1f / Time.unscaledDeltaTime)).ToString();
        }

        public virtual void MachineUpdate()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}
