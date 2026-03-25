using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
[CreateAssetMenu(fileName = "new item data", menuName = "data/item effect")]

public class itemeffect : ScriptableObject
{
    public virtual void excuteeffect(Transform _posision)
    {
        Debug.Log("effect excuted");
    }
    public virtual void removeeffect(Transform _posision)
    {
        Debug.Log("effect removed");
    }
    public virtual void excutehealeffect()
    {

    }

}
