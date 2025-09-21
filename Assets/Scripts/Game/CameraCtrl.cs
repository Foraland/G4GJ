using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    Camera cam;
    float Height => cam.orthographicSize * 2;
    float Width => Height * cam.aspect;
    public Transform follow;
    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        RoomCtrl room = GM.Ins.currentRoom;
        float x = Mathf.Clamp(follow.position.x, room.MinX + Width / 2, room.MaxX - Width / 2);
        float y = Mathf.Clamp(follow.position.y, room.MinY + Height / 2, room.MaxY - Height / 2);
        transform.position = new Vector3(x, y, -10);
    }
}
