using UnityEngine;

[System.Serializable]
public class inventoryitem
{
    public itemdata data;
    public int stacksize;

    public inventoryitem(itemdata _data)
    {
        data = _data;
        stacksize = 1;
    }

    public void Addstack(int amount = 1)
    {
        stacksize += amount;
    }

    public void Removestack(int amount = 1)
    {
        stacksize -= amount;
    }

    /// <summary>
    /// ÊÇ·ñ¿É¶Ñµþ
    /// </summary>
    public bool CanStack()
    {
        return data != null && data.canStack && stacksize < data.maxStackSize;
    }
}