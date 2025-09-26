using UnityEngine;

public class LandState : PlayerStateBase
{
    public LandState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        owner.PlayAnim("Land");
        owner.lockAnim = true;
        owner.anim.GetComponent<AnimStepSound>().finishLand += OnFinishLand;
    }
    public override void OnExit()
    {

        owner.anim.GetComponent<AnimStepSound>().finishLand += OnFinishLand;
    }
    private void OnFinishLand()
    {
        owner.lockAnim = false;
        owner.SwitchState(new CtrlState(owner));
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        owner.velocity = new Vector2(0, 0);
    }
}