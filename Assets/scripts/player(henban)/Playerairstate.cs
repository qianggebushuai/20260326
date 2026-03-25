using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerairstate : Playerstate
{
    public Playerairstate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }
    float currentgravity;
    float preparetime = 0.5f;
    public override void Enter()
    {
        base.Enter();
        preparetime = 0.1f;
        currentgravity = player.rb.gravityScale;
        if (Mathf.Abs(player.rb.velocity.x) > player.wallBounceSpeedH * 0.5f)
        {
            player.StartCoroutine(BounceControlDelay());
        }
    }

    private IEnumerator BounceControlDelay()
    {
        player.cancontrol = false;
        yield return new WaitForSeconds(0.1f); 
        player.cancontrol = true;
    }

    public override void Exit()
    {

        player.jumpchecktime = 0.3f;
        player.rb.gravityScale = currentgravity;
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        player.rb.gravityScale += 0.5f * Time.deltaTime;
        player.jumpchecktime -= Time.deltaTime;
        preparetime-= Time.deltaTime;

        player.Setvelocity(xInput, player.rb.velocity.y);


        if (player.isgrounddetected())
        {
            statemachine.changestate(player.idlestate);
        }
        if (player.iswalldetected())
        {
            statemachine.changestate(player.wallslide);
        }
        if (Input.GetKeyDown(KeyCode.Space) && player.candoublejump)
        {
            player.candoublejump = false;
            player.Setvelocity(0, player.rb.velocity.y);
            statemachine.changestate(player.jumpstate);

        }
        if (Input.GetKeyDown(KeyCode.Space) &&player.jumpchecktime>=0)
        {
            player.Setvelocity(0, player.rb.velocity.y);
            statemachine.changestate(player.jumpstate);
        }
    }
}
