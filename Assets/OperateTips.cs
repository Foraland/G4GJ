using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OperateTips : SingletonComp<OperateTips>
{
    public void FadeIn()
    {
        GetComponent<Image>().DOFade(1, 1);
    }
    public void FadeOut()
    {
        GetComponent<Image>().DOFade(0, 1);
    }
}
