using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerwalljump : Playerstate
{
    public Playerwalljump(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        player.candoublejump = true;
        stateTimer = .4f;
        player.rb.velocity = new Vector2(player.rb.velocity.x * 5*-player.facingdirection, player.jumpforce);
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.Setvelocity(xInput, player.rb.velocity.y);
        if (stateTimer<0)
        {
            statemachine.changestate(player.airstate);
        }
        if (player.isgrounddetected())
        {
            statemachine.changestate(player.idlestate);
        }
    }
}
