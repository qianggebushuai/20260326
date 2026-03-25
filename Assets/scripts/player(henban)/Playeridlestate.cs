using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playeridlestate : Playergroundedstate
{
    public Playeridlestate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.rb.velocity = new Vector2(0, player.rb.velocity.y);
    }

    
    public override void Update()
    {
        base.Update();
        if (xInput != 0)
        {
            statemachine.changestate(player.movestate);

        }
    }

    public override void Exit()
    {
        base.Exit();
    }


}
