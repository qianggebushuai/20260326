using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playergroundedstate : Playerstate
{
    public Playergroundedstate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.candoublejump = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        keycontrol();


    }
    public void keycontrol()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            statemachine.changestate(player.aimsword);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            statemachine.changestate(player.counterattack);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && player.isgrounddetected())
        {
            statemachine.changestate(player.primaryattack);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            statemachine.changestate(player.summonstate);
        }


        if (Input.GetKeyDown(KeyCode.Space) && player.isgrounddetected())
        {

            statemachine.changestate(player.jumpstate);
        }


        if (player.rb.velocity.y < 0 && !player.isgrounddetected())
        {
            statemachine.changestate(player.airstate);
        }


    }
}
