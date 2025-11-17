using UnityEngine;

public abstract class PlayerBaseState2
{
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState2 _currentSubState;
    private PlayerBaseState2 _currentSuperState;
    
    protected PlayerStateMachine Ctx => _ctx;
    protected PlayerStateFactory Factory => _factory;

    public PlayerBaseState2 CurrentSubState => _currentSubState;

    public PlayerBaseState2(PlayerStateMachine currenContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currenContext;
        _factory = playerStateFactory;

    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchState();
    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        Debug.Log(_currentSubState);
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    protected void SwitchState(PlayerBaseState2 newState)
    {
        ExitState();
        newState.EnterState();
        _ctx.CurrentActionState = newState;
    }

    protected void SetSuperState(PlayerBaseState2 newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseState2 newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}