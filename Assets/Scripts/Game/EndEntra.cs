using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEntra : ExitEntra
{
    protected override void OnTouchEntra()
    {
        GM.Ins.SetEnd();
    }
}
