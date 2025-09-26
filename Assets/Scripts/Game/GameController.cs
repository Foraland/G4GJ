using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class GameController : SingletonComp<GameController>
{
    public bool playEnd = false;
    public string startTxt = "在深山里安眠是向晨爷爷最后的夙愿\n当晓晴和向晨在深山里一起埋葬好爷爷的骨灰盒后\n已经是深夜了";
    public AudioFireCtrl fireAudio;
    public ParticleSystem firePs;
    public Image start1;
    public Image end;
    public CanvasGroup developer;
    public CanvasGroup endText;
    public void StartGame()
    {
        StartCoroutine(StartGameIE());
    }
    public IEnumerator WaitTrap()
    {
        bool flag = false;
        Action resolve = () => flag = true;
        GM.Ins.triggerTrapDanger += resolve;
        while (!flag)
            yield return null;
        GM.Ins.triggerTrapDanger -= resolve;
    }
    public IEnumerator WaitExitRoom()
    {
        bool flag = false;
        Action resolve = () => flag = true;
        GM.Ins.exitCurRoom += resolve;
        while (!flag)
            yield return null;
        GM.Ins.exitCurRoom -= resolve;
    }
    public IEnumerator WaitEnterRoom()
    {
        bool flag = false;
        Action resolve = () => flag = true;
        GM.Ins.enterNextRoom += resolve;
        while (!flag)
            yield return null;
        GM.Ins.enterNextRoom -= resolve;
    }

    public IEnumerator DialogGrp1()
    {
        yield return GameDialog.Ins.SetDialog(false, 2, "可别小看我的听力，我也是拿过学校听声辩位比赛一等奖的呢！", false, DialogEffect.JumpUp);

    }
    public IEnumerator StartGameIE()
    {
        GM.Ins.isInTransition = true;
        OperateTips.Ins.FadeOut();
        StoryBoard.Ins.EnterStory();
        yield return new WaitForSeconds(2);
        yield return start1.DOFade(0, 2);
        StartText.Ins.SetText(startTxt);
        yield return new WaitForSeconds(11);
        yield return GameDialog.Ins.SetDialog(true, 1, "......", false, DialogEffect.None);
        yield return GameDialog.Ins.SetDialog(false, 3, "向晨，你还好吗？", false, DialogEffect.BounceDown);
        yield return Fadding.Ins.FadeOut();
        yield return GameDialog.Ins.SetDialog(true, 0, "没事，就是在想…… 爷爷一辈子在轮椅上，他走后会有人记得他吗？", false, DialogEffect.JumpUp);
        yield return GameDialog.Ins.SetDialog(false, 1, "当然啦。爷爷创建了“星光残联会” ，大家会永远记得他，他变成天上的星星，宇宙会永远记得他。");
        yield return GameDialog.Ins.SetDialog(true, 1, "是啊……倒把这些忘了。", false, DialogEffect.BounceDown);
        yield return GameDialog.Ins.SetDialog(false, 3, "要不是爷爷带我来 “听山”，我可能还把自己锁在屋里。我出了车祸后，像被扔进了黑洞......", false, DialogEffect.Scare);
        yield return GameDialog.Ins.SetDialog(true, 0, "没事，都过去了。我们边走边说吧。天色很晚了，我们要下山了。", false, DialogEffect.Shift);
        yield return GameDialog.Ins.SetDialog(false, 0, "好");
        yield return StoryBoard.Ins.ExitStory();
        fireAudio.Ban();
        firePs.Stop();
        Player.Ins.ResumeCtrl();
        OperateTips.Ins.FadeIn();
        GM.Ins.isInTransition = false;

        yield return WaitTrap();
        Player.Ins.BanCtrl();
        OperateTips.Ins.FadeOut();
        StoryBoard.Ins.EnterStory();
        yield return GameDialog.Ins.SetDialog(false, 3, "向晨小心！！！", false, DialogEffect.Shake);
        yield return GameDialog.Ins.SetDialog(true, 3, "哇！吓死我了......幸好有你提醒，不然又要吃板栗了!");
        StoryBoard.Ins.ExitStory();
        Player.Ins.ResumeCtrl();
        OperateTips.Ins.FadeIn();

        Coroutine cor = StartCoroutine(DialogGrp1());
        yield return WaitExitRoom();
        StopCoroutine(cor);
        GameDialog.Ins.StopDialog();

        while (true)
        {
            if (playEnd)
                break;
            yield return null;
        }
        GM.Ins.isInTransition = true;
        StoryBoard.Ins.EnterStory();
        Compass.Ins.canvasGrp.DOFade(0, 2);
        Player.Ins.BanCtrl();
        yield return GameDialog.Ins.SetDialog(false, 0, "我好像“听”到光了。前面是。。。。。。");
        yield return GameDialog.Ins.SetDialog(true, 0, "是哦，我们......出来了。");
        yield return GameDialog.Ins.SetDialog(false, 0, "我现在突然想点一份超大号的披萨。");
        yield return GameDialog.Ins.SetDialog(true, 0, "披萨！我要芝士加倍，再加一份烤翅！");
        yield return GameDialog.Ins.SetDialog(false, 0, "好饿啊！快走吧，回家！");
        yield return Fadding.Ins.FadeIn();
        yield return end.DOFade(1, 3).WaitForCompletion();
        yield return new WaitForSeconds(4);
        yield return end.DOFade(0, 3).WaitForCompletion();
        yield return developer.DOFade(1, 3).WaitForCompletion();
        yield return new WaitForSeconds(3);
        yield return developer.DOFade(0, 3).WaitForCompletion();
        yield return endText.DOFade(1, 3).WaitForCompletion();
        yield return new WaitForSeconds(3);
        yield return endText.DOFade(0, 3).WaitForCompletion();
    }
}
