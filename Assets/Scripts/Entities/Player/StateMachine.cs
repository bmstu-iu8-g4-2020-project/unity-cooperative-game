using System.Collections.Generic;
using Entities.Player.States;

namespace Entities.Player
{
    public class StateMachine // mb change to Stack-Based FSM
    {
        public State CurrentState { get; private set; }

        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            startingState.OnEnter();
        }

        public void ChangeState(State newState)
        {
            CurrentState.OnExit();

            CurrentState = newState;
            newState.OnEnter();
        }

        # region State Queue

        // TODO create AutoActionState and only that state and its heir can store in queue. This state mean that OnEnd it call nextState from queue 
        private readonly Queue<State> _stateQueue = new Queue<State>();
        public void AddStateToQueue(State delayedState) => _stateQueue.Enqueue(delayedState);
        public void StartNextStateFromQueue() => ChangeState(_stateQueue.Dequeue());
        public void ClearQueue() => _stateQueue.Clear();
        public bool QueueEmpty() => _stateQueue.Count == 0;

        #endregion
    }
}
