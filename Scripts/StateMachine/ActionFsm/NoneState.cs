using System.Collections;
using UnityEngine;

public class NoneState : PlayerBaseState2
{
    
    public NoneState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }
    
    public override void EnterState()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {
        
    }
    
    public override void CheckSwitchState()
    {
        if (Ctx.IsAttackPressed && !Ctx.RequireNewAttackPress)
        {
            SwitchState(Factory.Attack());
        }
    }
}