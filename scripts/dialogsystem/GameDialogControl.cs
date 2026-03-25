using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
[System.Serializable]
public class CustomEvent : UnityEvent<string> { }
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
    public event Action<string> OnFlagSet;
    public event Action<string> OnFlagRemoved;
    public event Action<string> OnItemAdded;
    public event Action<string> OnItemRemoved;
    public event Action<string, int> OnTalkCountChanged;
    public CustomEvent onCustomEvent;
    private Dictionary<string, int> talkCounts = new Dictionary<string, int>();
    [SerializeField]private HashSet<string> globalFlags = new HashSet<string>();


    #endregion

    private void Start()
    {

    }

    private void Update()
    {
        UpdateDebugItems(); // 更新调试列表
    }

    /// <summary>
    /// 更新调试用的物品列表（同步 inventory 系统）
    /// </summary>
    private void UpdateDebugItems()
    {
        if (inventory.instance == null) return;

        debugItems.Clear();
        for (int i = 0; i < inventory.instance.TotalSize; i++)
        {
            inventoryitem item = inventory.instance.GetItemAtSlot(i);
            if (item != null && item.data != null)
            {
                debugItems.Add($"{item.data.itemname} x{item.stacksize}");
            }
        }
    }

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
        if (string.IsNullOrEmpty(itemId)) return false;
        if (inventory.instance == null) return false;

        for (int i = 0; i < inventory.instance.TotalSize; i++)
        {
            inventoryitem item = inventory.instance.GetItemAtSlot(i);
            if (item != null && item.data != null && item.data.itemname == itemId)
            {
                return true;
            }
        }
        return false;
    }

    public int GetItemCount(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return 0;
        if (inventory.instance == null) return 0;

        int count = 0;
        for (int i = 0; i < inventory.instance.TotalSize; i++)
        {
            inventoryitem item = inventory.instance.GetItemAtSlot(i);
            if (item != null && item.data != null && item.data.itemname == itemId)
            {
                count += item.stacksize;
            }
        }
        return count;
    }

    /// <summary>
    /// ★ 添加物品（使用 itemdata 引用）
    /// </summary>
    public void AddItem(itemdata itemData, int amount = 1)
    {
        if (itemData == null) return;
        if (inventory.instance == null)
        {
            Debug.LogWarning("[GameDialogControl] inventory.instance 为空，无法添加物品");
            return;
        }

        if (inventory.instance.AddItem(itemData, amount))
        {
            OnItemAdded?.Invoke(itemData.itemname);
            Debug.Log($"[GameDialogControl] 获得物品: {itemData.itemname} x{amount}");
        }
    }

    /// <summary>
    /// ★ 移除物品（使用 itemdata 引用）
    /// </summary>
    public void RemoveItem(itemdata itemData, int amount = 1)
    {
        if (itemData == null) return;
        if (inventory.instance == null)
        {
            Debug.LogWarning("[GameDialogControl] inventory.instance 为空，无法移除物品");
            return;
        }

        if (inventory.instance.RemoveItem(itemData, amount))
        {
            OnItemRemoved?.Invoke(itemData.itemname);
            Debug.Log($"[GameDialogControl] 移除物品: {itemData.itemname} x{amount}");
        }
    }

    /// <summary>
    /// ★ 通过物品名称移除物品（需要先查找）
    /// </summary>
    public void RemoveItemByName(string itemId, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemId)) return;
        if (inventory.instance == null) return;

        // 查找对应的 itemdata
        for (int i = 0; i < inventory.instance.TotalSize; i++)
        {
            inventoryitem item = inventory.instance.GetItemAtSlot(i);
            if (item != null && item.data != null && item.data.itemname == itemId)
            {
                RemoveItem(item.data, amount);
                return;
            }
        }
    }

    /// <summary>
    /// ★ 获取所有物品名称集合
    /// </summary>
    public HashSet<string> GetAllItems()
    {
        HashSet<string> items = new HashSet<string>();

        if (inventory.instance == null) return items;

        for (int i = 0; i < inventory.instance.TotalSize; i++)
        {
            inventoryitem item = inventory.instance.GetItemAtSlot(i);
            if (item != null && item.data != null)
            {
                items.Add(item.data.itemname);
            }
        }
        return items;
    }

    public Dictionary<string, int> GetAllItemsWithCount()
    {
        Dictionary<string, int> items = new Dictionary<string, int>();

        if (inventory.instance == null) return items;

        for (int i = 0; i < inventory.instance.TotalSize; i++)
        {
            inventoryitem item = inventory.instance.GetItemAtSlot(i);
            if (item != null && item.data != null)
            {
                string itemName = item.data.itemname;
                if (items.ContainsKey(itemName))
                {
                    items[itemName] += item.stacksize;
                }
                else
                {
                    items[itemName] = item.stacksize;
                }
            }
        }
        return items;
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
        onCustomEvent?.Invoke(eventName);
        Debug.Log($"[GameDialogControl] 触发事件: {eventName}");
    }

    #endregion

    #region 存档/读档（已更新）

    [System.Serializable]
    private class SaveData
    {
        public Dictionary<string, int> talkCounts;
        public List<string> flags;
    }

    public string SerializeToJson()
    {
        SaveData data = new SaveData
        {
            talkCounts = new Dictionary<string, int>(talkCounts),
            flags = new List<string>(globalFlags)
        };
        return JsonUtility.ToJson(data);
    }

    public void DeserializeFromJson(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data == null) return;

        talkCounts = data.talkCounts ?? new Dictionary<string, int>();
        globalFlags = new HashSet<string>(data.flags ?? new List<string>());

        debugFlags = new List<string>(globalFlags);
    }

    #endregion
}