using UnityEngine;

public class WaitState : PlayerStateBase
{
    public WaitState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        owner.PlayAnim("Idle");
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