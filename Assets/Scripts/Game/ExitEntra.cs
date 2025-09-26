using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitEntra : MonoBehaviour
{
    public RoomCtrl nextRoom;
    protected virtual void OnTouchEntra()
    {
        Player.Ins.ExitAllDanger();
        GM.Ins.ExitRoom();
        GM.Ins.ResetToCheckpoint(() =>
        {
            nextRoom.OnEnter();
            GM.Ins.EnterRoom();
        });
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            OnTouchEntra();
        }
    }
}
