using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameController
{
    public bool playEnd = false;
    const string startTxt = "在深山里安眠是向晨爷爷最后的夙愿\n当晓晴和向晨在深山里一起埋葬好爷爷的骨灰盒后\n已经是深夜了";
    public IEnumerator StartGame()
    {
        // yield return new WaitForSeconds(1);
        // StartText.Ins.SetText(startTxt);
        // yield return new WaitForSeconds(11);
        // yield return GameDialog.Ins.SetDialog(false, 0, "[手语]那是我失去光明后才拥有的世界。将注意力都交给耳朵，我感受到了鸟群迁徙时对远方的向往，风吹落叶时对大地的眷恋，还有蝉虫鸣叫时对生命的热情与绽放，那一刻我才发觉，看不见，也许不全是坏事。");
        // yield return Fadding.Ins.FadeOut();
        yield return null;
    }
}
