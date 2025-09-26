using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : SingletonComp<Friend>
{
    public Transform dustPoint;
    public float distance = 0.5f;
    public Animator anim;
    public SpriteRenderer sprr => anim.GetComponent<SpriteRenderer>();
    public bool isDbg = false;
    public GameObject dbgPrefab;
    [Header("只读")]
    public Vector2 velocity;
    public bool isInGround;
    public CapsuleCollider2D clider;
    [HideInInspector]
    public Rigidbody2D rb;
    // private Queue<Vector3> posQueue = new();
    private List<Vector3> posList = new();
    private List<GameObject> dbgList = new();
    private string currentAnimName = "Idle";
    private Vector3 lastPos;
    public bool isInWait = true;
    private bool isInLand = false;
    void PlayAnim(string animName, bool force = false)
    {
        if (isInLand)
            return;
        if (force)
        {
            currentAnimName = animName;
            anim.Play(animName);
            return;
        }
        if (currentAnimName == animName)
            return;
        if (animName == "Jump")
        {
            AddStepDust();
        }
        currentAnimName = animName;
        anim.Play(animName);
    }
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        lastPos = transform.position;
    }
    void Start()
    {
        Player.Ins.moveEvt += OnPlayerMove;
    }
    void Enqueue()
    {
        posList.Add(Player.Ins.transform.position);
        if (isDbg)
        {
            GameObject go = dbgPrefab.OPGet(GM.Ins.root.transform);
            go.transform.position = Player.Ins.transform.position;
            dbgList.Add(go);
        }
    }
    public void AddStepDust()
    {
        ParticleSystem dust = GlobalRef.Ins.dustPs.OPGet(transform.parent).GetComponent<ParticleSystem>();
        dust.transform.position = dustPoint.position;
        dust.Play();
        TM.SetTimer(dust.Hash("dust"), 2, null, s => dust.gameObject.OPPush());
    }
    Vector3 Dequeue()
    {
        Vector3 pos = posList[0];
        posList.RemoveAt(0);
        if (isDbg)
        {
            dbgList[0].OPPush();
            dbgList.RemoveAt(0);
        }
        return pos;
    }
    Vector3 Pop()
    {
        int lastIdx = posList.Count - 1;
        Vector3 pos = posList[lastIdx];
        posList.RemoveAt(lastIdx);
        if (isDbg)
        {
            dbgList[lastIdx].OPPush();
            dbgList.RemoveAt(lastIdx);
        }
        return pos;
    }
    Vector3 Peek()
    {
        return posList[posList.Count - 1];
    }
    bool IsAwayFromPlayer()
    {
        return Vector3.Distance(transform.position, Player.Ins.transform.position) > distance;
    }
    void OnPlayerMove()
    {
        while (posList.Count > 0 && isInGround && Vector3.Distance(Peek(), transform.position) > Vector3.Distance(Player.Ins.transform.position, transform.position))
            Pop();
        Enqueue();
    }
    private void OnDrawGizmos()
    {
        float x = transform.position.x + clider.offset.x;
        float y = transform.position.y - clider.size.y / 2 + clider.offset.y - 0.01f;
        float endx = transform.position.x + clider.size.x / 2 + clider.offset.x;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(x, y, 0), new Vector3(x, y - 1, 0));
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
                    PlayAnim("Land");
                    isInLand = true;
                    AddStepDust();
                    anim.GetComponent<AnimStepSound>().finishLand += OnFinishLand;
                }
                isInGround = true;
                flag = true;
                break;
            }
        }
        if (!flag)
            isInGround = false;

        bool isRight = anim.transform.parent.localScale.x > 0;
        Vector3 playerPos = Player.Ins.transform.position;
        if (isInGround)
        {
            float xDelta = transform.position.x - lastPos.x;
            if (xDelta > 0)
            {
                isRight = true;
                PlayAnim("Walk");
            }
            else if (xDelta < 0)
            {
                isRight = false;
                PlayAnim("Walk");
            }
            else if (playerPos.x - transform.position.x > 0)
            {
                isRight = true;
                PlayAnim("Idle");
            }
            else
            {
                isRight = false;
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
    private void OnFinishLand()
    {
        isInLand = false;
        anim.GetComponent<AnimStepSound>().finishLand -= OnFinishLand;
    }
    void FixedUpdate()
    {
        velocity = transform.position - lastPos;
        lastPos = transform.position;
        if (!isInLand && (IsAwayFromPlayer() || !isInGround) && posList.Count > 0)
        {
            Vector3 pos = Dequeue();
            // float deltaDist = Vector3.Distance(transform.position, pos);
            // if (deltaDist <= Player.Ins.moveSpeed * Mathf.Sqrt(2) * Time.fixedDeltaTime)
            // rb.MovePosition(pos);
            transform.SetPositionAndRotation(pos, Quaternion.identity);
        }

    }
}
