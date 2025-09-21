using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    protected bool isChecked = false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isChecked && collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isChecked = true;
            GM.Ins.checkPoint = this;
        }
    }
}
