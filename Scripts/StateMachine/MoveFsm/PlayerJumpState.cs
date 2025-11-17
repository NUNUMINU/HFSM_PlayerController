using System.Collections;
using Resources.Scripts.StateMachine;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        :base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }
    
    IEnumerator JumpResetRoutine()
    {
        yield return new WaitForSeconds(.5f);
        Ctx.JumpCount = 0;
    }
    
    
    public override void EnterState()
    {
        InitializeSubState();
        HandleJump();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchState();
    }

    public override void ExitState()
    {
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
        Ctx.Animator.SetBool("isJumping", false);
        
        Ctx.CurrenJumpResetRoutine = Ctx.StartCoroutine(JumpResetRoutine()); // 연속 점프를 위한 코루틴
        if (Ctx.JumpCount == 3)
        {
            Ctx.JumpCount = 0;
            Ctx.Animator.SetInteger("JumpCount", Ctx.JumpCount);
        }
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

    void HandleJump()
    {
        if (Ctx.JumpCount < 3 && Ctx.CurrenJumpResetRoutine != null)
        {
            Ctx.StopCoroutine(Ctx.CurrenJumpResetRoutine);
        }
        
        Ctx.IsJumping = true;
        Ctx.RequireNewJumpPress = true;
        Ctx.Animator.SetBool("isJumping", true);
            
        Ctx.JumpCount += 1;
        Ctx.Animator.SetInteger("JumpCount", Ctx.JumpCount);
            
        Ctx.MovementY = Ctx.InitialJumpVelocities[Ctx.JumpCount];
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocities[Ctx.JumpCount];
    }

    public void HandleGravity()
    {
        bool isFalling = Ctx.MovementY <= 0.0f;
                         //|| !Ctx.IsJumpPressed;
        float falMult = 2.0f;
        
        
        if (isFalling) // 떨어질 때 더 빠르게
        {
            float previousYVelocity = Ctx.MovementY; // 지난 프레임
            Ctx.MovementY = previousYVelocity + (Ctx.JumpGravities[Ctx.JumpCount] * Time.deltaTime * falMult); // 현재 프레임
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.MovementY) * 0.5f, -15.0f); // 프레임 보정
        }
        else
        {
            // Movement.y += gravity * Time.deltaTime;
            float previousYVelocity = Ctx.MovementY; 
            Ctx.MovementY = previousYVelocity + (Ctx.JumpGravities[Ctx.JumpCount] * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.MovementY) * 0.5f, -15.0f); // 프레임 보정
        }
    }
}
