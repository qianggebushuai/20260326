using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueCondition
{
    [Header("对话次数条件")]
    public string targetNPCId;                    // 和哪个NPC对话
    public int minTalkCount = 0;                  // 最少对话次数（>=）
    public int maxTalkCount = int.MaxValue;       // 最多对话次数（<=）

    [Header("物品持有条件")]
    public List<string> requiredItems = new List<string>();  // 需要持有的物品ID

    [Header("全局标记条件")]
    public List<string> requiredFlags = new List<string>();  // 需要的全局标记

    [Header("排除标记（如果有这些标记则不触发）")]
    public List<string> excludeFlags = new List<string>();

    /// <summary>
    /// 检查所有条件是否满足
    /// </summary>
    public bool IsMet()
    {
        GameDialogControl gdc = GameDialogControl.Instance;

        // 1. 检查对话次数
        if (!string.IsNullOrEmpty(targetNPCId))
        {
            int count = gdc.GetTalkCount(targetNPCId);
            if (count < minTalkCount || count > maxTalkCount)
                return false;
        }

        // 2. 检查必须持有的物品
        foreach (string itemId in requiredItems)
        {
            if (!gdc.HasItem(itemId))
                return false;
        }

        // 3. 检查必须拥有的标记
        foreach (string flag in requiredFlags)
        {
            if (!gdc.HasFlag(flag))
                return false;
        }

        // 4. 检查排除标记
        foreach (string flag in excludeFlags)
        {
            if (gdc.HasFlag(flag))
                return false;
        }

        return true;
    }
}