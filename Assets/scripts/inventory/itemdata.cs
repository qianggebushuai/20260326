using UnityEngine;

public enum itemtype
{
    material,
    handcatch  // 手持物品
}

[CreateAssetMenu(fileName = "new item data", menuName = "data/item")]
public class itemdata : ScriptableObject
{
    [Header("基础信息")]
    public string itemname;
    public Sprite icon;
    public itemtype itemtype;

    [Header("堆叠设置")]
    public bool canStack = true;      // 是否可堆叠
    public int maxStackSize = 64;     // 最大堆叠数量

    [Header("装备效果（可选）")]
    public itemeffect[] itemeffects;  // 装备效果

    /// <summary>
    /// 执行装备效果
    /// </summary>
    public virtual void ExecuteEquipEffect(Transform playerTransform)
    {
        if (itemeffects == null || itemeffects.Length == 0) return;

        foreach (var effect in itemeffects)
        {
            if (effect != null)
            {
                effect.excuteeffect(playerTransform);
            }
        }
    }

    /// <summary>
    /// 移除装备效果
    /// </summary>
    public virtual void RemoveEquipEffect(Transform playerTransform)
    {
        if (itemeffects == null || itemeffects.Length == 0) return;

        foreach (var effect in itemeffects)
        {
            if (effect != null)
            {
                effect.removeeffect(playerTransform);
            }
        }
    }
}