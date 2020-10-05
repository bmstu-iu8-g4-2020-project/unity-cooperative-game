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
}
