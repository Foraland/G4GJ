using UnityEngine;

public class Launch : MonoBehaviour
{
    public GameObject root;

    private void Awake()
    {
        GM.Ins.Init(root.transform);
    }
    void Start()
    {
        GM.Ins.OnStart();
    }
    private void Update()
    {
        TM.OnUpdate();
        GM.Ins.OnUpdate();
    }
}