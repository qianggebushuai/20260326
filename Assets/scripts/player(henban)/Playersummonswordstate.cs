using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plyersummonswordstate : Playerstate
{
    public Plyersummonswordstate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
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
        if (triggercalled)
        {
            statemachine.changestate(player.idlestate);
        }
    }
}
