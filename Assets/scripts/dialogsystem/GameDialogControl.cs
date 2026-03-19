using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 全局对话状态管理器（单例）
/// 存储：对话次数、物品持有、全局标记
/// </summary>
public class GameDialogControl : MonoBehaviour
{
    #region 单例
    private static GameDialogControl _instance;
    public static GameDialogControl Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameDialogControl>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("[GameDialogControl]");
                    _instance = go.AddComponent<GameDialogControl>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region 数据存储

    [Header("调试用 — 可在Inspector中查看")]
    [SerializeField] private List<string> debugFlags = new List<string>();
    [SerializeField] private List<string> debugItems = new List<string>();

    // NPC对话次数记录  key=npcId, value=对话次数
    private Dictionary<string, int> talkCounts = new Dictionary<string, int>();

    // 玩家持有物品  key=itemId
    private HashSet<string> inventory = new HashSet<string>();

    // 全局标记/Flag  key=flagName
    private HashSet<string> globalFlags = new HashSet<string>();

    #endregion

    #region 事件
    public event Action<string> OnFlagSet;
    public event Action<string> OnFlagRemoved;
    public event Action<string> OnItemAdded;
    public event Action<string> OnItemRemoved;
    public event Action<string, int> OnTalkCountChanged; // npcId, newCount
    public event Action<string> OnCustomEvent;           // 自定义事件
    #endregion

    #region 对话次数

    public int GetTalkCount(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return 0;
        talkCounts.TryGetValue(npcId, out int count);
        return count;
    }

    public void IncrementTalkCount(string npcId)
    {
        if (string.IsNullOrEmpty(npcId)) return;

        if (!talkCounts.ContainsKey(npcId))
            talkCounts[npcId] = 0;

        talkCounts[npcId]++;
        OnTalkCountChanged?.Invoke(npcId, talkCounts[npcId]);

        Debug.Log($"[GameDialogControl] {npcId} 对话次数: {talkCounts[npcId]}");
    }

    public void SetTalkCount(string npcId, int count)
    {
        talkCounts[npcId] = count;
        OnTalkCountChanged?.Invoke(npcId, count);
    }

    #endregion

    #region 物品系统

    public bool HasItem(string itemId)
    {
        return inventory.Contains(itemId);
    }

    public void AddItem(string itemId)
    {
        if (inventory.Add(itemId))
        {
            debugItems.Add(itemId);
            OnItemAdded?.Invoke(itemId);
            Debug.Log($"[GameDialogControl] 获得物品: {itemId}");
        }
    }

    public void RemoveItem(string itemId)
    {
        if (inventory.Remove(itemId))
        {
            debugItems.Remove(itemId);
            OnItemRemoved?.Invoke(itemId);
            Debug.Log($"[GameDialogControl] 移除物品: {itemId}");
        }
    }

    public HashSet<string> GetAllItems()
    {
        return new HashSet<string>(inventory);
    }

    #endregion

    #region 全局标记

    public bool HasFlag(string flagName)
    {
        return globalFlags.Contains(flagName);
    }

    public void SetFlag(string flagName)
    {
        if (globalFlags.Add(flagName))
        {
            debugFlags.Add(flagName);
            OnFlagSet?.Invoke(flagName);
            Debug.Log($"[GameDialogControl] 设置标记: {flagName}");
        }
    }

    public void RemoveFlag(string flagName)
    {
        if (globalFlags.Remove(flagName))
        {
            debugFlags.Remove(flagName);
            OnFlagRemoved?.Invoke(flagName);
            Debug.Log($"[GameDialogControl] 移除标记: {flagName}");
        }
    }

    #endregion

    #region 触发自定义事件

    public void TriggerCustomEvent(string eventName)
    {
        OnCustomEvent?.Invoke(eventName);
        Debug.Log($"[GameDialogControl] 触发事件: {eventName}");
    }

    #endregion

    #region 存档/读档（可选扩展）

    [System.Serializable]
    private class SaveData
    {
        public Dictionary<string, int> talkCounts;
        public List<string> inventory;
        public List<string> flags;
    }

    public string SerializeToJson()
    {
        SaveData data = new SaveData
        {
            talkCounts = new Dictionary<string, int>(talkCounts),
            inventory = new List<string>(inventory),
            flags = new List<string>(globalFlags)
        };
        return JsonUtility.ToJson(data);
    }

    public void DeserializeFromJson(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data == null) return;

        talkCounts = data.talkCounts ?? new Dictionary<string, int>();
        inventory = new HashSet<string>(data.inventory ?? new List<string>());
        globalFlags = new HashSet<string>(data.flags ?? new List<string>());

        debugFlags = new List<string>(globalFlags);
        debugItems = new List<string>(inventory);
    }

    #endregion
}