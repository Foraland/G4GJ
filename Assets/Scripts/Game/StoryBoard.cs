using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StoryBoard : SingletonComp<StoryBoard>
{
    public float time = 0.8f;
    public RectTransform top;
    public RectTransform bottom;
    public IEnumerator EnterStory()
    {
        top.DOAnchorPosY(-top.sizeDelta.y / 2, time).SetEase(Ease.OutQuad);
        yield return bottom.DOAnchorPosY(bottom.sizeDelta.y / 2, time).SetEase(Ease.OutQuad).WaitForCompletion();
    }
    public IEnumerator ExitStory()
    {
        top.DOAnchorPosY(top.sizeDelta.y / 2, time).SetEase(Ease.OutQuad);
        yield return bottom.DOAnchorPosY(-bottom.sizeDelta.y / 2, time).SetEase(Ease.OutQuad).WaitForCompletion();
    }
}
