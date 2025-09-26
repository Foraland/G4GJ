using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCtrl : MonoBehaviour
{
    public bool isFirst = false;
    public Transform dangerItemFolders;
    public float MinX => transform.position.x - transform.lossyScale.x / 2;
    public float MaxX => transform.position.x + transform.lossyScale.x / 2;
    public float MinY => transform.position.y - transform.lossyScale.y / 2;
    public float MaxY => transform.position.y + transform.lossyScale.y / 2;
    public string bgmName = "";
    public CheckPoint spawn;
    [Header("只读")]
    public List<DangerItem> dangerItems = new();
    protected virtual void Awake()
    {
        Scan(dangerItemFolders);
        if (isFirst)
        {
            GM.Ins.currentRoom = this;
            AudioManager.PlayMusic(bgmName);
            GM.Ins.checkPoint = spawn;
        }
    }
    void Scan(Transform root)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            DangerItem item = child.GetComponent<DangerItem>();
            if (item != null)
            {
                item.room = this;
                dangerItems.Add(item);
            }
            Scan(child);
        }

    }
    public void OnEnter()
    {
        GM.Ins.checkPoint = spawn;
        GM.Ins.currentRoom = this;
        AudioManager.PlayMusic(bgmName);
    }
}
