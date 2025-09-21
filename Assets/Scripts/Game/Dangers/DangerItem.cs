using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerItem : MonoBehaviour, IDangerTarget
{
    [Header("只读")]
    public RoomCtrl room;
    public bool hasResolved = false;
    public event Action<DangerItem> resolveAction = (s) => { };
    public Vector3 GetCenter()
    {
        return transform.position;
    }
    public void Resolve()
    {
        hasResolved = true;
        resolveAction.Invoke(this);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.Ins.OnTouchDanger(this);
        }
    }
}
