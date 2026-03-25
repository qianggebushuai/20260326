using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerstatemachine 
{
    public Playerstate currentstate { get; private set; }
    public void Initialized(Playerstate _startstate)
    {
        currentstate = _startstate;
        currentstate.Enter();
    }
    public void changestate(Playerstate _newstate)
    {
        currentstate.Exit();
        currentstate = _newstate;
        currentstate.Enter();
    }
}
