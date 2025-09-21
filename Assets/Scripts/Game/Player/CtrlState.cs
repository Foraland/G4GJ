using UnityEngine;
public class CtrlState : PlayerStateBase
{
    private bool isInMove = false;
    private bool isInJump = false;
    private float jumpTimer = 0;
    private float axis = 0;
    private Vector2 v => owner.velocity;
    public CtrlState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        owner.touchCeil += OnTouchCeil;
    }
    public override void OnExit()
    {
        base.OnExit();
        owner.touchCeil -= OnTouchCeil;
    }
    private void OnTouchCeil()
    {
        isInJump = false;
        owner.velocity.y = 0;
    }
    public override void OnUpdate()
    {
        // 水平轴输入 = 左右输入轴值 * 控制系数Buff * 混乱状态反向（true则乘-1）
        axis = Input.GetAxisRaw("Horizontal");
        // 跳跃输入检测：按下瞬间（对应is_action_just_pressed）
        bool isJumpStart = Input.GetKeyDown(KeyCode.Space);
        // 跳跃输入检测：按住状态（对应is_action_pressed）
        bool isInJumpInput = Input.GetKey(KeyCode.Space);
        // 移动输入检测：左/右键按住（对应is_action_pressed）
        isInMove = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        // 跳跃触发逻辑
        if (isJumpStart)
        {
            jumpTimer = 0f;
            // 地面状态：进入跳跃状态
            if (owner.isInGround)
            {
                isInJump = true;
            }
            // 触发跳跃后禁用跳跃权限
        }

        // 跳跃结束条件：超过最小跳跃时间且松开按键，或超过最大跳跃时间
        if (isInJump && (jumpTimer > owner.jumpTimeRange.x && !isInJumpInput || jumpTimer > owner.jumpTimeRange.y))
        {
            isInJump = false;
        }

        // 调用主角动画更新方法
        owner.UpdateAnim();



    }
    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        float delta = Time.fixedDeltaTime; // 物理帧时间间隔
        float vy = owner.velocity.y;

        // 行为控制判断：若无行为或行为允许控制
        // 地面状态：允许跳跃
        if (isInJump)
        {
            jumpTimer += delta;
            vy = owner.jumpSpeed;
        }
        else if (!owner.isInGround)
            vy = owner.velocity.y - delta * owner.gravity;
        else
            owner.isResetY = true;
        // 计算X轴速度（移动逻辑）
        float vx = v.x;
        bool isZeroVx = Mathf.Abs(vx) < 0.001f;

        if (isInMove)
        {
            // 加速：速度方向与输入一致或速度接近0
            if (isZeroVx || (axis * vx) > 0)
            {
                vx = MUtils.LinearAttack(vx + 0.01f * axis, owner.acceleration * delta * Mathf.Abs(axis), owner.moveSpeed);
            }
            // 减速：速度方向与输入相反
            else
            {
                vx = MUtils.LinearDecay(vx, owner.acceleration * delta * Mathf.Abs(axis));
            }
        }
        else
        {
            // 无输入时：地面/空中减速
            vx = owner.isInGround
                ? MUtils.LinearDecay(vx, owner.moveDampInGround * delta)
                : MUtils.LinearDecay(vx, owner.moveDampInAir * delta);
        }

        owner.velocity = new Vector2(vx, vy);

    }
}