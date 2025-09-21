using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginRoom : RoomCtrl
{
    protected override void Awake()
    {
        base.Awake();
        OnEnter();
    }
}
