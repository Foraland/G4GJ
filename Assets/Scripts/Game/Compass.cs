using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
public class DangerInfo
{
    public IDangerTarget target;
    public float alertTimer = 0;
    public DangerInfo(IDangerTarget target, float alertTime)
    {
        this.target = target;
        this.alertTimer = alertTime;
    }
}
public class Compass : SingletonComp<Compass>
{
    public CanvasGroup canvasGrp;
    public float alertTime = 0.7f;
    private List<DangerInfo> dangerInfos = new();
    public Transform farestPoint;
    public GameObject prefab;
    private float maxDist => farestPoint.GetComponent<RectTransform>().anchoredPosition.magnitude;
    private float fadeTimer = 0;
    private TweenerCore<float, float, FloatOptions> fadeTwn = null;
    public void EnterDanger(IDangerTarget target)
    {
        dangerInfos.Add(new DangerInfo(target, alertTime));
    }
    public void ExitDanger(IDangerTarget target)
    {
        dangerInfos.Remove(dangerInfos.Find(e => e.target == target));
    }
    void Update()
    {
        if (dangerInfos.Count == 0)
        {
            if (fadeTimer > 3)
            {
                if (fadeTwn == null)
                    fadeTwn = canvasGrp.DOFade(0, 1);
            }
            else
                fadeTimer += Time.deltaTime;
        }
        else
        {
            fadeTimer = 0;
            if (fadeTwn != null)
            {
                fadeTwn.Complete();
                canvasGrp.alpha = 1;
                fadeTwn = null;
            }
        }
        for (int i = 0; i < dangerInfos.Count; i++)
        {
            DangerInfo info = dangerInfos[i];
            info.alertTimer -= Time.deltaTime;
            if (info.alertTimer < 0)
            {
                info.alertTimer += alertTime;
                Vector3 vec = info.target.GetCenter() - Player.Ins.transform.position;
                float originDist = vec.magnitude;
                float norDist = Player.Ins.dangerTrigger.GetComponent<CircleCollider2D>().radius;
                Vector3 v = vec / Mathf.Max(originDist, norDist) * maxDist;
                Alert(v);
            }
        }
    }
    void Alert(Vector3 lp)
    {
        GameObject go = prefab.OPGet(transform);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = lp;
        rt.localScale = new Vector3(1, 1, 1);
        rt.DOScale(3, 0.5f).SetEase(Ease.OutQuad);
        Image img = go.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 255);
        img.DOFade(0, 0.7f).SetEase(Ease.InQuad).onComplete = () => go.OPPush();
    }
}
