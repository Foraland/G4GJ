using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friend : SingletonComp<Friend>
{
    public float distance = 0.5f;
    public bool isDbg = false;
    public GameObject dbgPrefab;
    [Header("只读")]
    public Vector2 velocity;
    public bool isInGround;
    [HideInInspector]
    public Rigidbody2D rb;
    // private Queue<Vector3> posQueue = new();
    private List<Vector3> posList = new();
    private List<GameObject> dbgList = new();
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
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
        float x = transform.position.x - transform.localScale.x / 2;
        float y = transform.position.y - transform.localScale.y / 2 - 0.01f;
        float endx = transform.position.x + transform.localScale.x / 2;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(x + Mathf.Epsilon, y, 0), new Vector3(endx - Mathf.Epsilon, y, 0));
    }
    void Update()
    {
        float x = transform.position.x - transform.localScale.x / 2;
        float y = transform.position.y - transform.localScale.y / 2 - 0.01f;
        RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(x + Mathf.Epsilon, y), new Vector2(1, 0), transform.localScale.x - Mathf.Epsilon);
        bool flag = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider != null && hit.collider.tag == "Floor")
            {
                // if (!isInGround)
                //     rb.MovePosition(transform.position + new Vector3(0, -1, 0));
                isInGround = true;
                flag = true;
                break;
            }
        }
        if (!flag)
            isInGround = false;
    }
    void FixedUpdate()
    {
        if ((IsAwayFromPlayer() || !isInGround) && posList.Count > 0)
        {
            Vector3 pos = Dequeue();
            // float deltaDist = Vector3.Distance(transform.position, pos);
            // if (deltaDist <= Player.Ins.moveSpeed * Mathf.Sqrt(2) * Time.fixedDeltaTime)
            // rb.MovePosition(pos);
            transform.SetPositionAndRotation(pos, Quaternion.identity);
        }

    }
}
