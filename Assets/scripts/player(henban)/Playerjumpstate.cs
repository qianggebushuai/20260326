using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerjumpstate : Playerstate
{
    public Playerjumpstate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        player.rb.velocity = new Vector2(player.rb.velocity.x * 5,player.jumpforce);
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.jumpchecktime -= Time.deltaTime;
        if (player.jumpchecktime >= 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {

                statemachine.changestate(player.jumpstate);
            }
        }
        if (player.jumpchecktime < 0)
        {
            if (Input.GetKeyDown(KeyCode.Space)&&player.candoublejump)
            {
                player.candoublejump = false;
                statemachine.changestate(player.jumpstate);

            }
        }
        player.Setvelocity(xInput, player.rb.velocity.y);
        if (player.rb.velocity.y < 0)
        {
            statemachine.changestate(player.airstate);
        }
    }
}
