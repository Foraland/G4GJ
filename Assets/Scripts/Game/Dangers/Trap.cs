using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : DangerItem
{
    public Transform resetPos;
    private bool isDropping = false;
    public float gravity = 200;
    private float vy = 0;
    public void Reset()
    {
        isDropping = false;
        transform.position = resetPos.position;
        hasResolved = false;
        vy = 0;
    }
    public void OnTrigger()
    {
        isDropping = true;
    }
    void FixedUpdate()
    {
        if (isDropping && !hasResolved)
        {
            vy -= Time.fixedDeltaTime * gravity;
            transform.position = transform.position + new Vector3(0, vy * Time.fixedDeltaTime);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (isDropping && !hasResolved)
            {
                Resolve();
                LandToBreak();
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.Ins.OnTouchDanger(this);
        }
    }
    void LandToBreak()
    {

    }
}
