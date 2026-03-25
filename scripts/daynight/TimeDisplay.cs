using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI 时间显示器
/// </summary>
public class TimeDisplay : MonoBehaviour
{
    [Header("UI 引用")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image dayNightIcon;

    [Header("图标")]
    [SerializeField] private Sprite dayIcon;
    [SerializeField] private Sprite nightIcon;

    private void Start()
    {
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnTimeChanged += UpdateDisplay;
        }
    }

    private void OnDestroy()
    {
        if (DayNightCycle.Instance != null)
        {
            DayNightCycle.Instance.OnTimeChanged -= UpdateDisplay;
        }
    }

    private void UpdateDisplay(float currentTime)
    {
        if (timeText != null)
        {
            timeText.text = DayNightCycle.Instance.GetTimeString();
        }

        if (dayNightIcon != null)
        {
            dayNightIcon.sprite = DayNightCycle.Instance.IsDay ? dayIcon : nightIcon;
        }
    }
}