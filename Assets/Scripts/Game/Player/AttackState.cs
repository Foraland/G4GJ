using UnityEngine;

public class AttackState : PlayerStateBase
{
    public AttackState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        owner.PlayAnim("Attack", true);
        owner.lockAnim = true;
        TM.SetTimer("AttackTimer", owner.chopClip.length, null, s => owner.SwitchState(new CtrlState(owner)));
    }
    public override void OnExit()
    {
        base.OnExit();
        owner.lockAnim = false;
        TM.SetEnd("AttackTimer", false);
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        owner.velocity.x = 0;
        if (!owner.isInGround)
            owner.velocity.y = owner.velocity.y - Time.fixedDeltaTime * owner.gravity;
        else
            owner.isResetVY = true;
    }
}