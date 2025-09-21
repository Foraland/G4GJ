using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throns : DangerItem
{
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.Ins.OnTouchDanger(this);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("AttackArea"))
        {
            Broken();
            Resolve();
        }
    }
    void Broken()
    {

    }
}
