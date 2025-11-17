using UnityEngine;

public abstract class PlayerBaseState
{
  private bool _isRootState = false;
  private PlayerStateMachine _ctx;
  private PlayerStateFactory _factory;
  private PlayerBaseState _currentSubState;
  private PlayerBaseState _currentSuperState;
  
  protected bool IsRootState { set => _isRootState = value; }
  protected PlayerStateMachine Ctx => _ctx;
  protected PlayerStateFactory Factory => _factory;

  public PlayerBaseState CurrentSubState => _currentSubState;

  public PlayerBaseState(PlayerStateMachine currenContext, PlayerStateFactory playerStateFactory)
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

  protected void SwitchState(PlayerBaseState newState)
  {
    ExitState();

    newState.EnterState();

    if (_isRootState)
    {
      _ctx.CurrentMoveState = newState;
    } else if (_currentSuperState != null)
    {
      _currentSuperState.SetSubState(newState);
    }
  }

  protected void SetSuperState(PlayerBaseState newSuperState)
  {
    _currentSuperState = newSuperState;
  }

  protected void SetSubState(PlayerBaseState newSubState)
  {
    _currentSubState = newSubState;
    newSubState.SetSuperState(this);
  }
}
