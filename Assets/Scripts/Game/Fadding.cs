using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Fadding : SingletonComp<Fadding>
{
    public Image img;
    public IEnumerator FadeIn(float time = 1.5f)
    {
        yield return img.DOFade(1, time).WaitForCompletion();
    }
    public IEnumerator FadeOut(float time = 1.5f)
    {
        yield return img.DOFade(0, time).SetEase(Ease.InQuad).WaitForCompletion();
    }
}
