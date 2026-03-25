using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerstate
{
    protected Playerstatemachine statemachine;
    protected Player1 player;
    private string animboolname;

    protected float xInput;
    protected float yInput;
    protected float stateTimer;
    protected bool triggercalled=false;
    public Playerstate(Player1 _player,Playerstatemachine _statemachine,string _animboolname)
    {
        this.player = _player;
        this.statemachine = _statemachine;
        this.animboolname = _animboolname;

    }
    public virtual void Enter()
    {
        player.anim.SetBool(animboolname, true);
        triggercalled = false;
    }
    public virtual void Update()
    {
        
        stateTimer -= Time.deltaTime;
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical"); 
        player.anim.SetFloat("yVelocity", player.rb.velocity.y);
    }
   
    public virtual void Exit()
    {
        player.anim.SetBool(animboolname, false);
    }
    public virtual void Animationfinshedtrigger()
    {
        triggercalled = true;
    }
}
