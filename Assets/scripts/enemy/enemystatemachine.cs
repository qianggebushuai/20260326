using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemystatemachine 
{
    public enemystate currentstate { get; private set; }
    public void Initialize(enemystate _startstate)
    {
        currentstate = _startstate;
        currentstate.Enter();
    }
    public void Changestate(enemystate _newstate)
    {
        currentstate.Exit();
        currentstate = _newstate;
        currentstate.Enter();
    }
}
