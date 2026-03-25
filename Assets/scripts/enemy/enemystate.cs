using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemystate
{
    protected enemystatemachine statemachine;
    protected enemy enemybase;
  
    protected string animboolname;
    protected float stateTimer;
    protected bool triggercalled;
    public enemystate(enemystatemachine _statemachine, enemy _enemybase,string _animboolname)
    {
        this.enemybase = _enemybase;
        this.statemachine = _statemachine;
        this.animboolname = _animboolname;
    }
   public virtual void Enter()
    {
        enemybase.anim.SetBool(animboolname, true);
        triggercalled = false;
    }
    public virtual void Exit()
    {
       
        enemybase.anim.SetBool(animboolname, false);
    }
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }
    public virtual void animationfinishtrigger()
    {
        triggercalled = true;
    }
}
