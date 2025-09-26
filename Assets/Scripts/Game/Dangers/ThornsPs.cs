using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsPs : MonoBehaviour
{
    public List<ParticleSystem> ps = new();
    public void Play()
    {
        ps.ForEach(e => e.Play());
        TM.SetTimer(this.Hash("ThronsPs"), 1, null, s => gameObject.OPPush());
    }
}
