using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        rt.DOScale(2, 0.5f).SetEase(Ease.OutQuad);
        Image sprr = go.GetComponent<Image>();
        sprr.color = new Color(sprr.color.r, sprr.color.g, sprr.color.b, 255);
        sprr.DOFade(0, 0.7f).SetEase(Ease.InQuad).onComplete = () => go.OPPush();
    }
}
