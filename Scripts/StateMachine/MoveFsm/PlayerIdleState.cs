using System.Collections;
using Resources.Scripts.StateMachine;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
        :base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Ctx.Animator.SetBool("isWalking", false);
        Ctx.Animator.SetBool("isRunning", false);
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
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
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed) SwitchState(Factory.Run());
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed) SwitchState(Factory.Walk());
    }
}
