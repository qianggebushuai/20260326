using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Stats 
{
    [SerializeField] private int basevalue;
    public int getvalue()
    {
        int finalvalue = basevalue;
        foreach(int modifier in modifiers)
        {
            finalvalue += modifier;
        }
        return finalvalue;
    }

    public void setdefaultvalue(int _value)
    {
        basevalue = _value;
    }
    public List<int> modifiers;
    public void Addmodifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }
    public void Removemodifier(int _modifier)
    {
        modifiers.Remove(_modifier);
    }

}
