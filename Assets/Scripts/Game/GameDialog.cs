using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
public class DialogInfo
{
    public bool isHero;
    public int faceIdx;
    public string message;
    public DialogEffect effect;
}
public enum DialogEffect
{
    None,
    Shift,
    Shake,
    JumpUp,
    BounceDown,
    Scare
}
public class GameDialog : SingletonComp<GameDialog>
{
    public List<Sprite> heroFaces = new();
    public List<Sprite> friendFaces = new();
    private Coroutine txtCor;
    private Queue<DialogInfo> queue = new();
    [Header("组件")]
    public DialogItem heroDialog;
    public DialogItem friendDialog;
    private WaitForSeconds textInterval;
    private WaitForSeconds endWait;
    private RectTransform RT => transform as RectTransform;
    void Start()
    {
        textInterval = new WaitForSeconds(0.05f);
        endWait = new WaitForSeconds(1.5f);
    }
    public IEnumerator SetDialog(bool isHero, int faceIdx, string message, bool force = false, DialogEffect effect = DialogEffect.Shift)
    {
        if (force)
        {
            if (txtCor != null)
                StopCoroutine(txtCor);
            txtCor = null;
            queue.Clear();
        }
        queue.Enqueue(new DialogInfo
        {
            isHero = isHero,
            faceIdx = faceIdx,
            message = message,
            effect = effect
        });
        if (txtCor == null && queue.Count > 0)
        {
            txtCor = StartCoroutine(DialogIE());
        }
        yield return txtCor;
    }
    public void StopDialog()
    {
        queue.Clear();
        if (txtCor != null)
        {
            StopCoroutine(txtCor);
        }
        HideDialog();
    }
    public IEnumerator WaitCurrentDialog()
    {
        if (txtCor == null)
            yield break;
        yield return txtCor;
    }
    public void HideDialog()
    {
        heroDialog.gameObject.SetActive(false);
        friendDialog.gameObject.SetActive(false);
    }
    private IEnumerator DialogIE()
    {
        if (queue.Count <= 0)
        {
            HideDialog();
            txtCor = null;
            yield break;
        }
        DialogInfo info = queue.Dequeue();
        switch (info.effect)
        {
            case DialogEffect.BounceDown:
                BounceDown();
                break;
            case DialogEffect.JumpUp:
                JumpUp();
                break;
            case DialogEffect.Shake:
                Shake();
                break;
            case DialogEffect.Shift:
                Shift();
                break;
            case DialogEffect.Scare:
                Scare();
                break;
        }
        heroDialog.gameObject.SetActive(info.isHero);
        friendDialog.gameObject.SetActive(!info.isHero);
        DialogItem target = info.isHero ? heroDialog : friendDialog;
        List<Sprite> faces = info.isHero ? heroFaces : friendFaces;
        List<char> charList = new(info.message);
        string str = "";
        for (int i = 0; i < charList.Count; i++)
        {
            str += charList[i];
            target.SetData(faces[info.faceIdx], str);
            yield return textInterval;
        }
        yield return endWait;
        yield return DialogIE();
    }
    public void Shake()
    {
        RT.DOShakeAnchorPos(0.2f, 40, 40, 90, false, false);
    }
    public void Scare()
    {
        RT.DOShakeAnchorPos(0.6f, 7, 50, 90, false, false);
    }
    public void JumpUp()
    {
        Vector2 anchorPos = RT.anchoredPosition;
        RT.DOAnchorPos(new Vector2(anchorPos.x, anchorPos.y + 20), 0.05f).onComplete = () =>
        {
            RT.DOAnchorPos(anchorPos, 0.05f);
        };
    }
    public void BounceDown()
    {
        Vector2 anchorPos = RT.anchoredPosition;
        RT.DOAnchorPos(new Vector2(anchorPos.x, anchorPos.y - 20), 0.15f).SetEase(Ease.OutQuad).onComplete = () =>
        {
            RT.DOAnchorPos(anchorPos, 0.15f).SetEase(Ease.InQuad);
        };
    }
    public void Shift()
    {
        Vector2 anchorPos = RT.anchoredPosition;
        RT.DOAnchorPos(new Vector2(anchorPos.x, anchorPos.y - 10), 0.05f).onComplete = () =>
        {
            RT.DOAnchorPos(anchorPos, 0.05f);
        };
    }
}
