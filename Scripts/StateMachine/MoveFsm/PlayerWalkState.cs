using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        :base(currentContext, playerStateFactory)
    {
    }
    public override void EnterState()
    {
        Ctx.Animator.SetBool("isWalking", true);
        Ctx.Animator.SetBool("isRunning", false);
    }

    public override void UpdateState()
    {
        Ctx.AppliedMovementX = Ctx.Movement.x * Ctx.MovSpeed;
        Ctx.AppliedMovementZ = Ctx.Movement.y * Ctx.MovSpeed;
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
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed) {SwitchState(Factory.Run());}
        else if (!Ctx.IsMovementPressed) {SwitchState(Factory.Idle());}
    }
}
