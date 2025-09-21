using UnityEngine;

public class AttackState : PlayerStateBase
{
    public AttackState(Player player) : base(player)
    {
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        owner.velocity.x = 0;
        if (!owner.isInGround)
            owner.velocity.y = owner.velocity.y - Time.fixedDeltaTime * owner.gravity;
        else
            owner.isResetY = true;
    }
}