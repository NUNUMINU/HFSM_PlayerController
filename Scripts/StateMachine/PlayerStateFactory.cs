using System.Collections.Generic;
using UnityEngine;

public class PlayerStateFactory
{
    enum PlayerStates
    {
        idle, walk, run, grounded, fall, jump, none, attack
    }
    private PlayerStateMachine _ctx;
    private Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();
    private Dictionary<PlayerStates, PlayerBaseState2> _states2 = new Dictionary<PlayerStates, PlayerBaseState2>();
    
    public PlayerStateFactory(PlayerStateMachine currenContext)
    {
        // Move
        _ctx = currenContext;
        _states[PlayerStates.idle] = new PlayerIdleState(_ctx, this);
        _states[PlayerStates.walk] = new PlayerWalkState(_ctx, this);
        _states[PlayerStates.run] = new PlayerRunState(_ctx, this);
        _states[PlayerStates.grounded] = new PlayerGroundedState(_ctx, this);
        _states[PlayerStates.jump] = new PlayerJumpState(_ctx, this);
        _states[PlayerStates.fall] = new PlayerFallState(_ctx, this);
        
        // Action
        _states2[PlayerStates.none] = new NoneState(_ctx, this);
        _states2[PlayerStates.attack] = new PlayerAttackState(_ctx, this);
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }

    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.walk];
    }

    public PlayerBaseState Run()
    {
        return _states[PlayerStates.run];
    }

    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.jump];
    }

    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }
    
    public PlayerBaseState Fall()
    {
        return _states[PlayerStates.fall];
    }
    
    public PlayerBaseState2 None()
    {
        return _states2[PlayerStates.none];
    }
    
    public PlayerBaseState2 Attack()
    {
        return _states2[PlayerStates.attack];
    }
}
