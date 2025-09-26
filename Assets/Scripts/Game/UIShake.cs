using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class UIShaket : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalPosition;

    [Header("震动设置")]
    [Tooltip("震动幅度")]
    public float shakeAmount = 5f;
    [Tooltip("震动频率")]
    public float shakeSpeed = 20f;
    [Tooltip("震动持续时间")]
    public float shakeDuration = 0.5f;

    private bool isShaking = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
    }

    /// <summary>
    /// 开始震动效果
    /// </summary>
    public void StartShake()
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine());
        }
    }

    /// <summary>
    /// 开始自定义参数的震动效果
    /// </summary>
    /// <param name="amount">震动幅度</param>
    /// <param name="speed">震动频率</param>
    /// <param name="duration">持续时间</param>
    public void StartShake(float amount, float speed, float duration)
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeCoroutine(amount, speed, duration));
        }
    }

    /// <summary>
    /// 震动协程
    /// </summary>
    private IEnumerator ShakeCoroutine()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 计算随机偏移量
            float x = originalPosition.x + Random.Range(-shakeAmount, shakeAmount);
            float y = originalPosition.y + Random.Range(-shakeAmount, shakeAmount);

            // 应用位置偏移
            rectTransform.localPosition = new Vector3(x, y, originalPosition.z);

            // 累加时间并等待下一帧
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // 震动结束后回到原始位置
        rectTransform.localPosition = originalPosition;
        isShaking = false;
    }

    /// <summary>
    /// 带自定义参数的震动协程
    /// </summary>
    private IEnumerator ShakeCoroutine(float amount, float speed, float duration)
    {
        isShaking = true;
        float elapsed = 0f;
        float currentTime = 0f;

        while (elapsed < duration)
        {
            currentTime += Time.deltaTime * speed;

            // 使用正弦函数创建更有规律的震动效果
            float x = originalPosition.x + Mathf.Sin(currentTime) * amount * Random.Range(0.5f, 1f);
            float y = originalPosition.y + Mathf.Cos(currentTime) * amount * Random.Range(0.5f, 1f);

            rectTransform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        rectTransform.localPosition = originalPosition;
        isShaking = false;
    }
}
