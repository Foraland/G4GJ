using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : SingletonComp<Player>
{
    public AnimationClip chopClip;
    public bool lockAnim = false;
    public Transform dustPoint;
    public float moveSpeed = 5f;
    public Vector2 jumpSpeedRange = new Vector2(14, 25);
    public float maxSpeedTime = 0.6f;
    public Transform attackArea;
    public float acceleration = 100;
    public float moveDampInGround = 30;
    public float gravity = 50;
    public event Action moveEvt = () => { };
    public Transform chargeBarBg;
    public Transform chargeBarFill;
    public CapsuleCollider2D clider;
    public Animator anim;
    public DangerTrigger dangerTrigger;
    [Header("只读")]
    public Vector2 velocity;
    public bool isInGround;
    [HideInInspector]
    public Rigidbody2D rb;
    public PlayerStateBase currentState;
    [HideInInspector]
    public bool isResetVY = false;
    public event Action touchCeil = () => { };
    public event Action land = () => { };
    private string currentAnimName = "Idle";
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    public void PlayAnim(string animName, bool force = false)
    {
        if (lockAnim)
            return;
        if (force)
        {
            currentAnimName = animName;
            anim.Play(animName);
            return;
        }
        if (currentAnimName == animName)
            return;
        currentAnimName = animName;
        anim.Play(animName);
    }
    void Start()
    {
        BanCtrl();
    }
    public void AddStepDust()
    {
        ParticleSystem dust = GlobalRef.Ins.dustPs.OPGet(transform.parent).GetComponent<ParticleSystem>();
        dust.transform.position = dustPoint.position;
        dust.Play();
        TM.SetTimer(dust.Hash("dust"), 2, null, s => dust.gameObject.OPPush());
    }
    public void ShowChargeBar()
    {
        chargeBarBg.parent.gameObject.SetActive(true);
    }
    public void HideChargeBar()
    {
        chargeBarBg.parent.gameObject.SetActive(false);
    }
    public void SetChargeProgress(float progress)
    {
        chargeBarFill.localScale = new Vector3(chargeBarBg.localScale.x, chargeBarBg.localScale.y * progress, 1);
        chargeBarFill.position = new Vector3(chargeBarBg.position.x, chargeBarBg.position.y - (1 - progress) * chargeBarBg.localScale.y / 2);
        MUtils.SetGOAlpha(chargeBarBg.parent, progress);
    }
    public List<DangerItem> dangerItems = new();
    public void UpdateAnim()
    {

        bool isRight = anim.transform.parent.localScale.x > 0;
        if (isInGround)
        {
            if (velocity.x > 0.1)
            {
                isRight = true;
                PlayAnim("Walk");
            }
            else if (velocity.x < -0.1)
            {
                isRight = false;
                PlayAnim("Walk");
            }
            else
            {
                PlayAnim("Idle");
            }
        }
        else
        {
            if (velocity.y > 0)
                PlayAnim("Jump");
            else if (velocity.y < 0)
                PlayAnim("Fall");

        }
        anim.transform.parent.localScale = new Vector3(isRight ? 1 : -1, 1, 1);
    }
    public void BanCtrl()
    {
        SwitchState(new WaitState(this));
    }
    public void ResumeCtrl()
    {
        SwitchState(new CtrlState(this));
    }
    public void SwitchState(PlayerStateBase state)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = state;
        currentState.OnEnter();
    }
    private void OnDrawGizmos()
    {
        float x = transform.position.x - clider.size.x / 2 + clider.offset.x;
        float y = transform.position.y - clider.size.y / 2 + clider.offset.y + Mathf.Min(-0.01f, velocity.y * Time.deltaTime / 2);
        float endx = transform.position.x + clider.size.x / 2 + clider.offset.x;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(x + Mathf.Epsilon, y, 0), new Vector3(endx - Mathf.Epsilon, y, 0));
    }
    void Update()
    {
        float x = transform.position.x - clider.size.x / 2 + clider.offset.x;
        float y = transform.position.y - clider.size.y / 2 + clider.offset.y + Mathf.Min(-0.01f, velocity.y * Time.deltaTime / 2);
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(x + Mathf.Epsilon, y), new Vector2(1, 0), clider.size.x - Mathf.Epsilon);
        bool flag = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                if (!isInGround)
                {
                    rb.MovePosition(transform.position + new Vector3(0, -1, 0));
                    land.Invoke();
                    isResetVY = true;
                }
                isInGround = true;
                flag = true;
                break;
            }
        }
        if (!flag)
            isInGround = false;
        currentState.OnUpdate();
    }
    void FixedUpdate()
    {
        if (isResetVY)
        {
            isResetVY = false;
            velocity.y = 0;
        }
        currentState.OnFixedUpdate();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector3 pos = rb.position + velocity * Time.fixedDeltaTime;
        pos.x = Mathf.Max(GM.Ins.currentRoom.MinX + transform.localScale.x / 2, pos.x);
        rb.MovePosition(pos);
        if ((!isInGround && velocity.magnitude > 0.1) ||
        (isInGround && Math.Abs(velocity.x) > 0.1))
            moveEvt.Invoke();
    }
    public void OnDangerEnter(DangerItem item)
    {
        dangerItems.Add(item);
        if (!item.hasResolved)
        {
            item.resolveAction += OnResolveDanger;
            Compass.Ins.EnterDanger(item);
        }
    }
    private void OnResolveDanger(DangerItem item)
    {
        item.resolveAction -= OnResolveDanger;
        Compass.Ins.ExitDanger(item);
    }
    public void OnDangerExit(DangerItem item)
    {
        dangerItems.Remove(item);
        OnResolveDanger(item);

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                ContactPoint2D point = collision.contacts[i];

                float ang = Vector2.Angle(point.normal, new Vector2(0, -1));
                if (ang < 30)
                {
                    touchCeil.Invoke();
                }
            }
        }
    }
    public void OnTouchDanger(DangerItem item)
    {
        if (!item.hasResolved)
        {
            GM.Ins.OnPlayerTouchDanger();
        }
    }
    public void ExitAllDanger()
    {
        dangerItems.ForEach(e => e.Resolve());
    }
}
