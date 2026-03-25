using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercounterattackstate : Playerstate
{
    public Playercounterattackstate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        
        base.Enter();
        stateTimer = player.counterattackduration;
        player.anim.SetBool("Successfulcounterattack", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.Attackcheck.position, player.Attackcheckradios);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<enemy>() != null)
            {
                if (hit.GetComponent<enemy>().isstunned())
                {
                    stateTimer = 10f;
                    player.anim.SetBool("Successfulcounterattack", true);
                   
                }


            }
        }
        if (stateTimer < 0 || triggercalled)
        {
            statemachine.changestate(player.idlestate);
        }
    }
}
