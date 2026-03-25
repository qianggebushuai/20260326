using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 昼夜循环系统
/// 控制全局光照、环境颜色、时间流逝
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    public static DayNightCycle Instance { get; private set; }

    [Header("时间设置")]
    [SerializeField] private float dayDurationInMinutes = 10f;  // 一天的真实时长（分钟）
    [SerializeField] private float startTime = 6f;              // 开始时间（0-24小时）

    [Header("当前时间（只读）")]
    [SerializeField] private float currentTime = 12f;           // 当前时间（0-24）
    [SerializeField] private int currentDay = 1;                // 当前天数

    [Header("全局光照")]
    [SerializeField] private Light2D globalLight;               // 全局2D光源

    [Header("光照曲线")]
    [SerializeField] private Gradient lightColorGradient;       // 光照颜色渐变
    [SerializeField] private AnimationCurve lightIntensityCurve;// 光照强度曲线

    [Header("环境颜色")]
    [SerializeField] private SpriteRenderer skyRenderer;        // 天空背景
    [SerializeField] private Gradient skyColorGradient;         // 天空颜色渐变

    [Header("调试")]
    [SerializeField] private bool pauseTime = false;
    [SerializeField] private float timeScale = 1f;              // 时间流速倍率

    // 时间事件
    public System.Action<float> OnTimeChanged;                  // 时间改变
    public System.Action OnSunrise;                             // 日出
    public System.Action OnNoon;                                // 正午
    public System.Action OnSunset;                              // 日落
    public System.Action OnMidnight;                            // 午夜

    private float timeSpeed;
    private bool hasFiredSunrise, hasFiredNoon, hasFiredSunset, hasFiredMidnight;

    // 公共属性
    public float CurrentTime => currentTime;
    public float TimeNormalized => currentTime / 24f;
    public bool IsNight => currentTime < 6f || currentTime > 18f;
    public bool IsDay => !IsNight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentTime = startTime;
        timeSpeed = 24f / (dayDurationInMinutes * 60f);

        InitializeGradients();
        UpdateLighting();
    }

    private void Update()
    {
        if (pauseTime) return;

        // 更新时间
        float previousTime = currentTime;
        currentTime += Time.deltaTime * timeSpeed * timeScale;

        // 一天结束，进入新的一天
        if (currentTime >= 24f)
        {
            currentTime = 0f;
            currentDay++;
            ResetDayEvents();
        }

        // 更新光照
        UpdateLighting();

        // 触发时间事件
        CheckTimeEvents(previousTime, currentTime);

        OnTimeChanged?.Invoke(currentTime);
    }

    /// <summary>
    /// 初始化默认渐变曲线
    /// </summary>
    private void InitializeGradients()
    {
        if (lightColorGradient == null || lightColorGradient.colorKeys.Length == 0)
        {
            // 默认光照颜色渐变
            GradientColorKey[] colorKeys = new GradientColorKey[6];
            colorKeys[0] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 0.0f);    // 午夜 - 深蓝
            colorKeys[1] = new GradientColorKey(new Color(1f, 0.6f, 0.4f), 0.25f);     // 日出 - 橙红
            colorKeys[2] = new GradientColorKey(Color.white, 0.5f);                    // 正午 - 白色
            colorKeys[3] = new GradientColorKey(new Color(1f, 0.9f, 0.7f), 0.625f);    // 下午 - 暖黄
            colorKeys[4] = new GradientColorKey(new Color(1f, 0.5f, 0.3f), 0.75f);     // 日落 - 橙色
            colorKeys[5] = new GradientColorKey(new Color(0.1f, 0.1f, 0.3f), 1.0f);    // 夜晚 - 深蓝

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 1f);

            lightColorGradient = new Gradient();
            lightColorGradient.SetKeys(colorKeys, alphaKeys);
        }

        if (lightIntensityCurve == null || lightIntensityCurve.keys.Length == 0)
        {
            // 默认强度曲线
            lightIntensityCurve = new AnimationCurve();
            lightIntensityCurve.AddKey(0f, 0.1f);     // 午夜 - 10%
            lightIntensityCurve.AddKey(0.25f, 0.6f);  // 日出 - 60%
            lightIntensityCurve.AddKey(0.5f, 1.0f);   // 正午 - 100%
            lightIntensityCurve.AddKey(0.75f, 0.6f);  // 日落 - 60%
            lightIntensityCurve.AddKey(1f, 0.1f);     // 午夜 - 10%
        }

        if (skyColorGradient == null || skyColorGradient.colorKeys.Length == 0)
        {
            // 默认天空颜色
            GradientColorKey[] skyKeys = new GradientColorKey[5];
            skyKeys[0] = new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 0f);     // 夜晚
            skyKeys[1] = new GradientColorKey(new Color(1f, 0.5f, 0.3f), 0.25f);       // 日出
            skyKeys[2] = new GradientColorKey(new Color(0.53f, 0.81f, 0.92f), 0.5f);   // 白天
            skyKeys[3] = new GradientColorKey(new Color(1f, 0.4f, 0.2f), 0.75f);       // 日落
            skyKeys[4] = new GradientColorKey(new Color(0.05f, 0.05f, 0.15f), 1f);     // 夜晚

            skyColorGradient = new Gradient();
            skyColorGradient.SetKeys(skyKeys, new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            });
        }
    }

    /// <summary>
    /// 更新光照和环境
    /// </summary>
    private void UpdateLighting()
    {
        float normalizedTime = currentTime / 24f;

        // 更新全局光照
        if (globalLight != null)
        {
            globalLight.color = lightColorGradient.Evaluate(normalizedTime);
            globalLight.intensity = lightIntensityCurve.Evaluate(normalizedTime);
        }

        // 更新天空颜色
        if (skyRenderer != null)
        {
            skyRenderer.color = skyColorGradient.Evaluate(normalizedTime);
        }
    }

    /// <summary>
    /// 检查并触发时间事件
    /// </summary>
    private void CheckTimeEvents(float previousTime, float newTime)
    {
        // 日出 (6:00)
        if (previousTime < 6f && newTime >= 6f && !hasFiredSunrise)
        {
            hasFiredSunrise = true;
            OnSunrise?.Invoke();
            Debug.Log("[DayNight] 日出");
        }

        // 正午 (12:00)
        if (previousTime < 12f && newTime >= 12f && !hasFiredNoon)
        {
            hasFiredNoon = true;
            OnNoon?.Invoke();
            Debug.Log("[DayNight] 正午");
        }

        // 日落 (18:00)
        if (previousTime < 18f && newTime >= 18f && !hasFiredSunset)
        {
            hasFiredSunset = true;
            OnSunset?.Invoke();
            Debug.Log("[DayNight] 日落");
        }

        // 午夜 (0:00)
        if (previousTime < 24f && newTime >= 0f && !hasFiredMidnight)
        {
            hasFiredMidnight = true;
            OnMidnight?.Invoke();
            Debug.Log("[DayNight] 午夜");
        }
    }

    private void ResetDayEvents()
    {
        hasFiredSunrise = false;
        hasFiredNoon = false;
        hasFiredSunset = false;
        hasFiredMidnight = false;
    }

    #region 公共方法

    /// <summary>
    /// 设置时间
    /// </summary>
    public void SetTime(float hour)
    {
        currentTime = Mathf.Clamp(hour, 0f, 24f);
        UpdateLighting();
    }

    /// <summary>
    /// 暂停/恢复时间
    /// </summary>
    public void TogglePause()
    {
        pauseTime = !pauseTime;
    }

    /// <summary>
    /// 设置时间流速
    /// </summary>
    public void SetTimeScale(float scale)
    {
        timeScale = scale;
    }

    /// <summary>
    /// 获取时间字符串（HH:MM格式）
    /// </summary>
    public string GetTimeString()
    {
        int hours = Mathf.FloorToInt(currentTime);
        int minutes = Mathf.FloorToInt((currentTime - hours) * 60f);
        return $"{hours:D2}:{minutes:D2}";
    }

    #endregion
}