using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartText : SingletonComp<StartText>
{
    public Text text;
    public void SetText(string txt)
    {
        text.text = txt;
        text.DOFade(1, 3).onComplete = () =>
        {
            TM.SetTimer("startTxt", 3, null, (s) =>
            {
                text.DOFade(0, 3);
            });
        };
    }
}
