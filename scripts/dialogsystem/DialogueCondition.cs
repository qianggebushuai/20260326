using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueCondition
{
    [Header("对话次数条件")]
    public string targetNPCId;
    public int minTalkCount = 0;                 
    public int maxTalkCount = int.MaxValue;       

    [Header("物品持有条件")]
    public List<string> requiredItems = new List<string>(); 

    [Header("全局标记条件")]
    public List<string> requiredFlags = new List<string>(); 

    [Header("排除标记（如果有这些标记则不触发）")]
    public List<string> excludeFlags = new List<string>();

    public bool IsMet()
    {
        GameDialogControl gdc = GameDialogControl.Instance;

        if (!string.IsNullOrEmpty(targetNPCId))
        {
            int count = gdc.GetTalkCount(targetNPCId);
            if (count < minTalkCount || count > maxTalkCount)
                return false;
        }

        foreach (string itemId in requiredItems)
        {
            if (!gdc.HasItem(itemId))
                return false;
        }

        foreach (string flag in requiredFlags)
        {
            if (!gdc.HasFlag(flag))
                return false;
        }

        foreach (string flag in excludeFlags)
        {
            if (gdc.HasFlag(flag))
                return false;
        }

        return true;
    }
}