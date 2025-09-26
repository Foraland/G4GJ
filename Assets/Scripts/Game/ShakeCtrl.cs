using UnityEditor;
using UnityEngine;

public class ShakeCtrl : MonoBehaviour
{
    [Range(0.01f, 1f)] public float shakeAmount = 0.2f;

    [Range(1f, 100f)] public float shakeSpeed = 40;


    public float duration = 0.1f;

    private Vector3 _originalPosition;
    private bool _isShaking = false;
    private float _shakeTimer = 0f;

    private void Start()
    {
        // 记录初始位置
        _originalPosition = transform.localPosition;

    }

    private void Update()
    {
        if (_isShaking)
        {
            // 计算左右震动的偏移量（只在X轴上震动）
            float shakeOffset = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            transform.localPosition = new Vector3(
                _originalPosition.x + shakeOffset,
                _originalPosition.y,
                _originalPosition.z
            );

            // 如果设置了持续时间，计时并结束震动
            if (duration > 0)
            {
                _shakeTimer += Time.deltaTime;
                if (_shakeTimer >= duration)
                {
                    StopShaking();
                }
            }
        }
    }

    /// <summary>
    /// 开始震动
    /// </summary>
    public void StartShaking()
    {
        _isShaking = true;
        _shakeTimer = 0f;
    }

    /// <summary>
    /// 停止震动并回到初始位置
    /// </summary>
    public void StopShaking()
    {
        _isShaking = false;
        transform.localPosition = _originalPosition;
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(ShakeCtrl))]
    class ShakeCtrlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var obj = (ShakeCtrl)target;
            if (GUILayout.Button("TestBtn"))
            {
                obj.StartShaking();
            }
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj.gameObject);
        }
    }
#endif
}
