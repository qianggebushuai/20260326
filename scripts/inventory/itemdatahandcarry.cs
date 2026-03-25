using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum handcarrytype
{
   light,
   weapon
}
[CreateAssetMenu(fileName = "new item data", menuName = "data/handcarry")]
public class itemdatahandcarry: itemdata
{
    public handcarrytype carrytype;
    public List<inventoryitem> craftingMaterials;
    public void excuteitemeffect(Transform _enemyposision)
    {
        foreach (var item in itemeffects)
        {
            item.excuteeffect(_enemyposision);
        }
    }
    public void removeitemeffect(Transform _enemyposision)
    {
        foreach (var item in itemeffects)
        {
            item.removeeffect(_enemyposision);
        }
    }
}
