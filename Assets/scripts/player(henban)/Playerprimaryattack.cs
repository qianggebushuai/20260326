using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerprimaryattack : Playerstate
{
    public int combocounter;
    private float lasttimeattacked;
    private float combowindow=2;
    public Playerprimaryattack(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (combocounter > 2||Time.deltaTime>=combowindow+lasttimeattacked)
        {
            combocounter = 0;
        }
     
        player.anim.SetInteger("combocounter",combocounter);
        player.Setvelocity(player.Attackmovement[combocounter]*player.facingdirection,player.rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        combocounter++;
        lasttimeattacked+= Time.deltaTime;
    }

    public override void Update()
    {
        base.Update();
        if (triggercalled)
        {
            statemachine.changestate(player.idlestate);
        }
        if (stateTimer < 0)
        {
            player.rb.velocity=new Vector2(0,0);
        }


    }
}
