using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 2D 点光源控制器
/// 用于火把、灯笼、窗户光等夜晚光源
/// </summary>
[RequireComponent(typeof(Light2D))]
public class LightSource2D : MonoBehaviour
{
    [Header("光源设置")]
    [SerializeField] private bool autoToggleWithTime = true;   // 自动根据时间开关
    [SerializeField] private float turnOnHour = 18f;            // 开灯时间
    [SerializeField] private float turnOffHour = 6f;            // 关灯时间

    [Header("光源效果")]
    [SerializeField] private bool enableFlicker = true;         // 启用闪烁
    [SerializeField] private float flickerSpeed = 5f;           // 闪烁速度
    [SerializeField] private float flickerAmount = 0.2f;        // 闪烁幅度（0-1）

    [Header("光源颜色")]
    [SerializeField] private Gradient colorOverTime;            // 颜色随时间变化
    [SerializeField] private bool useColorGradient = false;

    private Light2D lightSource;
    private float baseIntensity;
    private Color baseColor;
    private float flickerTimer;

    private void Awake()
    {
        lightSource = GetComponent<Light2D>();
        baseIntensity = lightSource.intensity;
        baseColor = lightSource.color;
    }

    private void Start()
    {
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnTimeChanged += OnTimeChanged;
            UpdateLightState(DayNightCycle.Instance.CurrentTime);
        }
    }

    private void OnDestroy()
    {
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnTimeChanged -= OnTimeChanged;
        }
    }

    private void Update()
    {
        if (lightSource.enabled && enableFlicker)
        {
            ApplyFlicker();
        }
    }

    /// <summary>
    /// 时间变化回调
    /// </summary>
    private void OnTimeChanged(float currentTime)
    {
        if (autoToggleWithTime)
        {
            UpdateLightState(currentTime);
        }

        if (useColorGradient && colorOverTime != null)
        {
            float normalizedTime = currentTime / 24f;
            lightSource.color = colorOverTime.Evaluate(normalizedTime);
        }
    }

    /// <summary>
    /// 更新光源开关状态
    /// </summary>
    private void UpdateLightState(float currentTime)
    {
        bool shouldBeOn = false;

        if (turnOnHour > turnOffHour)
        {
            // 正常情况：18:00开，6:00关
            shouldBeOn = currentTime >= turnOnHour || currentTime < turnOffHour;
        }
        else
        {
            // 特殊情况：跨越午夜
            shouldBeOn = currentTime >= turnOnHour && currentTime < turnOffHour;
        }

        if (lightSource.enabled != shouldBeOn)
        {
            lightSource.enabled = shouldBeOn;
        }
    }

    /// <summary>
    /// 应用闪烁效果
    /// </summary>
    private void ApplyFlicker()
    {
        flickerTimer += Time.deltaTime * flickerSpeed;

        // 使用Perlin噪声创建自然闪烁
        float flicker = Mathf.PerlinNoise(flickerTimer, 0f);
        flicker = Mathf.Lerp(1f - flickerAmount, 1f + flickerAmount, flicker);

        lightSource.intensity = baseIntensity * flicker;
    }

    #region 公共方法

    /// <summary>
    /// 手动开启光源
    /// </summary>
    public void TurnOn()
    {
        lightSource.enabled = true;
        autoToggleWithTime = false;
    }

    /// <summary>
    /// 手动关闭光源
    /// </summary>
    public void TurnOff()
    {
        lightSource.enabled = false;
        autoToggleWithTime = false;
    }

    /// <summary>
    /// 设置光源强度
    /// </summary>
    public void SetIntensity(float intensity)
    {
        baseIntensity = intensity;
        lightSource.intensity = intensity;
    }

    #endregion
}