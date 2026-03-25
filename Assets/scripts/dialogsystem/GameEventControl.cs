using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public class GameEventControl : MonoBehaviour
{
    public List<eventandname> events;
    public static GameDialogControl instance;
    void Start()
    {
        
    }

    public void findeventsyhroughname(string _name) {
        for (int i = 0; i < events.Count; i++)
        {
            if (_name == events[i]._name)
            {
                events[i]._event.Invoke();
            }
        }
    }
}

