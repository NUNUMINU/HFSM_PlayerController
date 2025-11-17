using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        :base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.Animator.SetBool("isFall", true);
        InitializeSubState();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchState();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool("isFall", false);
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed)
        {
            SetSubState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        } else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
    }

    public override void CheckSwitchState()
    {
        if (Ctx.cc.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    void HandleGravity()
    {
        float previousYVelocity = Ctx.MovementY; // 지난 프레임
        Ctx.MovementY = previousYVelocity + (Ctx.Gravity * Time.deltaTime); // 현재 프레임
        Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.MovementY) * 0.5f, -15.0f); // 프레임 보정
    }
}
