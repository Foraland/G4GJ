using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFireCtrl : MonoBehaviour
{
    private AudioSource audioSource;
    public ParticleSystem ps;
    public float maxDist = 8;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public bool isActive = true;
    public void Ban()
    {
        isActive = false;
        audioSource.Stop();
        ps.gameObject.SetActive(false);
    }
    void Update()
    {
        if (!isActive)
            return;
        float dist = Vector3.Distance(Player.Ins.transform.position, transform.position);
        audioSource.volume = Mathf.Lerp(0, 1, 1 - Mathf.Min(1, dist / maxDist));
    }
}
