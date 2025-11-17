using System.Collections;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState2
{

    private float _attackDuration = 0.666f; // 공격 애니메이션 시간
    private float _elapsedTime;

    IEnumerator AttackResetRoutine()
    {
        yield return new WaitForSeconds(1.0f);
        Ctx.AttackCount = 0;
    }
    
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }
    
    public override void EnterState()
    {
        HandleAttack();
    }

    public override void UpdateState()
    {
        _elapsedTime += Time.deltaTime;
        CheckSwitchState();
        Debug.Log(_elapsedTime);
    }

    public override void ExitState()
    {
        if (Ctx.IsAttackPressed)
        {
            Ctx.RequireNewAttackPress = true;
        }
        
        Ctx.Animator.SetBool("isAttack", false);
        Ctx.CurrenAttackResetRoutine = Ctx.StartCoroutine(AttackResetRoutine()); // 연속 공격을 위한 코루틴
        
        if (Ctx.AttackCount == 3)
        {
            Ctx.AttackCount = 0;
            Ctx.Animator.SetInteger("AttackCount", Ctx.AttackCount);
        }
    }

    public override void InitializeSubState()
    {
    }
    
    public override void CheckSwitchState()
    {
        if (_elapsedTime >= _attackDuration)
        {
            SwitchState(Factory.None());
        }
    }

    void HandleAttack()
    {
        if (Ctx.AttackCount < 3 && Ctx.CurrenAttackResetRoutine != null)
        {
            Ctx.StopCoroutine(Ctx.CurrenAttackResetRoutine);
        }
        Ctx.AttackCount += 1;
        Ctx.Animator.SetInteger("AttackCount", Ctx.AttackCount);
        
        _elapsedTime = 0f;
        Ctx.Animator.SetBool("isAttack", true);
    }
}
