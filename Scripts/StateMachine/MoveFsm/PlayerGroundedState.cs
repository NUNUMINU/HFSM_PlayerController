using Resources.Scripts.StateMachine;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
   public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
      :base(currentContext, playerStateFactory)
   {
      IsRootState = true;
   }

   public void HandleGravity()
   {
      Ctx.MovementY = Ctx.GroundedGravity;
      Ctx.AppliedMovementY = Ctx.GroundedGravity;
   }

   public override void EnterState()
   {
      InitializeSubState();
      HandleGravity();
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
      if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
      {
         SetSubState(Factory.Walk());
      } else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
      {
         SetSubState(Factory.Run());
      } else
      {
         SetSubState(Factory.Idle());
      }
   }

   public override void CheckSwitchState()
   {
      // 점프 버튼 누르면 -> jump state
      if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress && Ctx.CurrentActionState is not PlayerAttackState) {SwitchState(Factory.Jump());}
      else if (!Ctx.cc.isGrounded) { SwitchState(Factory.Fall()); }
   }
}
