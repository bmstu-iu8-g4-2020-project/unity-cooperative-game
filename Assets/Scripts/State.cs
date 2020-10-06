using UnityEngine;
using UnityEngine.UI;

public abstract class State
{
    protected readonly Character Character;
    protected readonly StateMachine StateMachine;

    private Text _fpsText;

    protected State(Character character, StateMachine stateMachine)
    {
        Character = character;
        StateMachine = stateMachine;
    }

    public virtual void OnEnter()
    {
        _fpsText = GameManager.Instance.FPSText;
        GameManager.Instance.StateText.text = GetType().Name;
    }

    public virtual void ProcessInput()
    {
    }

    public virtual void MachineUpdate()
    {
    }

    public virtual void Tick()
    {
        _fpsText.text = ((int) (1f / Time.unscaledDeltaTime)).ToString();
    }

    public virtual void OnExit()
    {
    }
}
