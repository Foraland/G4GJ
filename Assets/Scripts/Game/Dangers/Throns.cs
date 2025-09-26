using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throns : DangerItem
{
    public int hp = 3;
    public SpriteRenderer sprr;
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.Ins.OnTouchDanger(this);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("AttackArea"))
        {
            OnChop();
        }
    }
    void OnChop()
    {
        if (hasResolved)
            return;
        hp--;
        GameObject psGO = GlobalRef.Ins.thornsPsPrefab.OPGet(transform.parent);
        psGO.transform.position = transform.position;
        psGO.GetComponent<ThornsPs>().Play();

        sprr.GetComponent<ShakeCtrl>().StartShaking();
        if (hp >= 0)
            AudioManager.PlaySFX("cut_branch_" + (3 - hp));
        if (hp <= 0)
        {
            Resolve();
            sprr.gameObject.SetActive(false);
        }
    }
}
