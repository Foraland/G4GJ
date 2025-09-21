using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : SingletonComp<Player>
{
    public float moveSpeed = 5f;
    public float jumpSpeed = 14;
    public Vector2 jumpTimeRange = new(0.01f, 0.1f);
    public float maxFallSpeed = 15;
    public float acceleration = 100;
    public float moveDampInGround = 30;
    public float moveDampInAir = 30;
    public float gravity = 50;
    public event Action moveEvt = () => { };
    public DangerTrigger dangerTrigger;
    [Header("只读")]
    public Vector2 velocity;
    public bool isInGround;
    [HideInInspector]
    public Rigidbody2D rb;
    public PlayerStateBase currentState;
    [HideInInspector]
    public bool isResetY = false;
    public event Action touchCeil = () => { };
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        ExitStory();
    }
    public List<DangerItem> dangerItems = new();
    public void UpdateAnim()
    {

    }
    public void EnterStory()
    {
        SwitchState(new WaitState(this));
    }
    public void ExitStory()
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
        float x = transform.position.x - transform.localScale.x / 2;
        float y = transform.position.y - transform.localScale.y / 2 + Mathf.Min(-0.01f, velocity.y * Time.deltaTime / 2);
        float endx = transform.position.x + transform.localScale.x / 2;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(x + Mathf.Epsilon, y, 0), new Vector3(endx - Mathf.Epsilon, y, 0));
    }
    void Update()
    {
        float x = transform.position.x - transform.localScale.x / 2;
        float y = transform.position.y - transform.localScale.y / 2 + Mathf.Min(-0.01f, velocity.y * Time.deltaTime / 2);
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(x + Mathf.Epsilon, y), new Vector2(1, 0), transform.localScale.x - Mathf.Epsilon);
        bool flag = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                if (!isInGround)
                    rb.MovePosition(transform.position + new Vector3(0, -1, 0));
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
        if (isResetY)
        {
            isResetY = false;
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
