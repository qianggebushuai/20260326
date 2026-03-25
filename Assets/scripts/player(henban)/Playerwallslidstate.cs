using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerwallslidstate : Playerstate
{
    public Playerwallslidstate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        player.candoublejump = true;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !player.isgrounddetected())
        {
            statemachine.changestate(player.walljump);
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Wall Dash - Diagonal Jump");

            player.wallDashDirection = -player.facingdirection;
            player.isWallDashing = true;

            player.Flip();

            statemachine.changestate(player.dashstate);
            return;
        }

        if (xInput != 0 && player.facingdirection != xInput)
        {
            statemachine.changestate(player.idlestate);
        }

        if (player.isgrounddetected())
        {
            statemachine.changestate(player.idlestate);
        }

        if (!player.iswalldetected())
        {
            statemachine.changestate(player.airstate);
        }

        if (yInput < 0)
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y);
        }
        else
        {
            player.rb.velocity = new Vector2(0, player.rb.velocity.y * 0.5f);
        }
        base.Update();
    }
}