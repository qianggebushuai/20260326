using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[CreateAssetMenu(fileName = "new event", menuName = "event/item")]
public class eventandname : ScriptableObject
{
    public string _name;
    public UnityEvent _event;
}

