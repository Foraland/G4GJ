using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerTrigger : MonoBehaviour
{
    public Player owner;
    void OnTriggerEnter2D(Collider2D collision)
    {
        DangerItem item = collision.GetComponent<DangerItem>();
        if (item != null && item.room == GM.Ins.currentRoom)
        {
            owner.OnDangerEnter(item);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        DangerItem item = collision.GetComponent<DangerItem>();
        if (item != null && item.room == GM.Ins.currentRoom)
        {
            owner.OnDangerExit(item);
        }
    }
}
