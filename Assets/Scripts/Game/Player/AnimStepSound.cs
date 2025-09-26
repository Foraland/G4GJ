using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimStepSound : MonoBehaviour
{
    readonly List<string> grass = new()
    {
        "grass1",
        "grass2",
        "grass3",
        "grass4"
    };
    readonly List<string> branch = new()
    {
        "branch1",
        "branch2",
        "branch3",
        "branch4"
    };
    public event Action finishLand = () => { };
    public void PlaySFX()
    {
        string sfxName = (GM.Ins.isGrass ? grass : branch).GetRandom();
        AudioManager.PlaySFX(sfxName, 0.5f);
    }
    public void Crop()
    {
        Player.Ins.attackArea.gameObject.SetActive(true);
        Player.Ins.attackArea.localScale = new Vector3(Player.Ins.anim.transform.parent.localScale.x, 1, 1);
        TM.SetTimer("ChopTimer", 0.1f, null, (s) =>
        {
            Player.Ins.attackArea.gameObject.SetActive(false);
        });
    }
    public void FinishLand()
    {
        finishLand.Invoke();
    }
}
