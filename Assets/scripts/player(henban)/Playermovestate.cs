using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermovestate : Playergroundedstate

{
    public Playermovestate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.Setvelocity(xInput,player.rb.velocity.y);
        if (xInput == 0)
        {
            statemachine.changestate(player.idlestate);

        }
    }
}
