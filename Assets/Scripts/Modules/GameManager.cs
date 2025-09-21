using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GM : Singleton<GM>
{
    public Transform root;
    public CheckPoint checkPoint;
    private GameController ctrl;
    private MonoBehaviour mono;
    private Vector3 checkPos => checkPoint.transform.position;
    private Coroutine resetCor = null;
    public RoomCtrl currentRoom;
    public void Init(Transform root)
    {
        this.root = root;
        ctrl = new GameController();
        mono = root.GetComponent<MonoBehaviour>();
    }
    public void SetEnd()
    {
        ctrl.playEnd = true;
    }
    public void OnStart()
    {
        mono.StartCoroutine(ctrl.StartGame());
    }
    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            OnPlayerTouchDanger();
    }
    public IEnumerator ExitStory()
    {
        Compass.Ins.canvasGrp.DOFade(1, StoryBoard.Ins.time);
        yield return StoryBoard.Ins.ExitStory();
    }
    public IEnumerator EnterStory()
    {
        Compass.Ins.canvasGrp.DOFade(0, StoryBoard.Ins.time);
        yield return StoryBoard.Ins.EnterStory();
    }
    public IEnumerator SetDialog(bool isHero, int faceIdx, string message, bool force = false)
    {
        yield return GameDialog.Ins.SetDialog(isHero, faceIdx, message, force);
    }
    public Coroutine ResetToCheckpoint(Action midCb = null)
    {
        if (resetCor != null)
            return resetCor;
        resetCor = mono.StartCoroutine(ResetToCheckpointIE(midCb));
        return resetCor;
    }
    public void OnPlayerTouchDanger()
    {
        ResetToCheckpoint(() =>
        {
            currentRoom.dangerItems.ForEach(e =>
            {
                if (e is Trap t)
                    t.Reset();
            });
        });
    }
    IEnumerator ResetToCheckpointIE(Action midCb)
    {
        Player.Ins.EnterStory();
        yield return Fadding.Ins.FadeIn();
        midCb?.Invoke();
        Player.Ins.rb.position = checkPos;
        Friend.Ins.rb.position = new Vector3(checkPos.x - Friend.Ins.distance, checkPos.y, 0);
        yield return Fadding.Ins.FadeOut();
        Player.Ins.ExitStory();
        resetCor = null;
    }
}
