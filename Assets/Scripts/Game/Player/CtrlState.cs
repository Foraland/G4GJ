using System;
using UnityEngine;
public class CtrlState : PlayerStateBase
{
    private bool isInMove = false;
    private float chargeTime = 0;
    private float axis = 0;
    private Vector2 v => owner.velocity;
    private float jumpV_hor = 0;
    private bool isInCharge = false;
    public CtrlState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        owner.touchCeil += OnTouchCeil;
        owner.land += OnLand;
    }
    public override void OnExit()
    {
        base.OnExit();
        owner.touchCeil -= OnTouchCeil;
        owner.land -= OnLand;
        owner.HideChargeBar();
    }
    private void OnLand()
    {
        AudioManager.PlaySFX("jump_2");
        owner.SwitchState(new LandState(owner));
        owner.AddStepDust();
    }
    private void OnTouchCeil()
    {
        owner.velocity.y = 0;
    }
    public override void OnUpdate()
    {
        axis = Input.GetAxisRaw("Horizontal");
        isInMove = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        if (Input.GetKeyDown(KeyCode.Mouse0))
            owner.SwitchState(new AttackState(owner));
        // 跳跃触发逻辑
        if (Input.GetKeyDown(KeyCode.Space) && owner.isInGround)
        {
            isInCharge = true;
            chargeTime = 0;
            owner.PlayAnim("Crouch");
            owner.lockAnim = true;
            jumpV_hor = 0;
            owner.ShowChargeBar();
        }
        if (owner.isInGround && !isInCharge && !TM.IsRunning("JumpUp"))
            jumpV_hor = owner.velocity.x;
        if ((Input.GetKeyUp(KeyCode.Space) || chargeTime > owner.maxSpeedTime) && isInCharge)
        {
            owner.HideChargeBar();
            owner.lockAnim = false;
            owner.PlayAnim("Jump");
            owner.AddStepDust();
            owner.lockAnim = true;
            if (Input.GetKey(KeyCode.D))
                jumpV_hor = owner.moveSpeed;
            else if (Input.GetKey(KeyCode.A))
                jumpV_hor = -owner.moveSpeed;
            TM.SetTimer("JumpUp", 0.05f, null, s => owner.lockAnim = false);
            AudioManager.PlaySFX("jump_1");
            isInCharge = false;
            owner.velocity.y = Mathf.Lerp(owner.jumpSpeedRange.x, owner.jumpSpeedRange.y, Mathf.Clamp01(chargeTime / owner.maxSpeedTime));
            chargeTime = 0;

        }
        if (isInCharge)
        {
            chargeTime += Time.deltaTime;
            owner.SetChargeProgress(chargeTime / owner.maxSpeedTime);
        }


        // 调用主角动画更新方法
        owner.UpdateAnim();
    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        float delta = Time.fixedDeltaTime; // 物理帧时间间隔
        float vy = v.y;
        float vx = v.x;

        // 行为控制判断：若无行为或行为允许控制
        // 地面状态：允许跳跃
        if (!owner.isInGround)
        {
            vy = owner.velocity.y - delta * owner.gravity;
            vx = jumpV_hor;
        }
        else
        {
            // 计算X轴速度（移动逻辑）
            bool isZeroVx = Mathf.Abs(vx) < 0.001f;
            if (isInCharge)
                vx = 0;
            else if (isInMove)
            {
                if (isZeroVx || (axis * vx) > 0)
                    vx = MUtils.LinearAttack(vx + 0.01f * axis, owner.acceleration * delta * Mathf.Abs(axis), owner.moveSpeed);
                else
                    vx = MUtils.LinearDecay(vx, owner.acceleration * delta * Mathf.Abs(axis));
            }
            else
                vx = MUtils.LinearDecay(vx, owner.moveDampInGround * delta);
        }
        owner.velocity = new Vector2(vx, vy);

    }
}