using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class DialogInfo
{
    public bool isHero;
    public int faceIdx;
    public string message;
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
    void Start()
    {
        textInterval = new WaitForSeconds(0.05f);
        endWait = new WaitForSeconds(1.5f);
    }
    public IEnumerator SetDialog(bool isHero, int faceIdx, string message, bool force = false)
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
        });
        if (txtCor == null && queue.Count > 0)
        {
            txtCor = StartCoroutine(DialogIE());
        }
        yield return txtCor;
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
}
