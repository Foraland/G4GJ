using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogItem : MonoBehaviour
{
    public Image face;
    public Text text;
    public void SetData(Sprite faceSprite, string str)
    {
        face.sprite = faceSprite;
        text.text = str;
    }
}
