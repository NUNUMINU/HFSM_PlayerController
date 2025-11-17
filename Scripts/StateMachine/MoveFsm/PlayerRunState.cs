using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        :base(currentContext, playerStateFactory)
    {
    }
    public override void EnterState()
    {
        Ctx.Animator.SetBool("isWalking", true);
        Ctx.Animator.SetBool("isRunning", true);

    }

    public override void UpdateState()
    {
        Ctx.AppliedMovementX = Ctx.Movement.x * Ctx.RunMult * Ctx.MovSpeed;
        Ctx.AppliedMovementZ = Ctx.Movement.y * Ctx.RunMult * Ctx.MovSpeed;
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
        if (Ctx.IsMovementPressed && !Ctx.IsRunPressed) SwitchState(Factory.Walk());
        else if (!Ctx.IsMovementPressed) SwitchState(Factory.Idle());
    }
}
