using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerdashstate : Playerstate
{
    private bool isVerticalDash;
    private float wallDashHorizontalSpeed;
    private float wallDashVerticalSpeed;
    
    public Playerdashstate(Player1 _player, Playerstatemachine _statemachine, string _animboolname) : base(_player, _statemachine, _animboolname)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.isdashing = true;
        
        if (player.isWallDashing)
        {
            stateTimer = player.wallDashDuration; 
            
            wallDashHorizontalSpeed = player.wallDashDirection * player.wallDashSpeedH;
            wallDashVerticalSpeed = player.wallDashSpeedV;
            
            player.rb.velocity = new Vector2(wallDashHorizontalSpeed, wallDashVerticalSpeed);
            
            Debug.Log($"Wall Dash Start: H={wallDashHorizontalSpeed}, V={wallDashVerticalSpeed}");
        }
        // X + Shift 垂直向上冲刺
        else if (Input.GetKey(KeyCode.X) && Input.GetKey(KeyCode.LeftShift))
        {
            isVerticalDash = true;
            stateTimer = player.dashduration;
            player.rb.velocity = new Vector2(0, player.dashspeed);
        }
        // 普通水平冲刺
        else
        {
            isVerticalDash = false;
            stateTimer = player.dashduration;
            
        }
    }

    public override void Exit()
    {
        player.isdashing = false;
        player.isWallDashing = false;
        isVerticalDash = false;
        base.Exit();
        
        if (player.isWallDashing)
        {
            player.rb.velocity = new Vector2(player.rb.velocity.x * 0.5f, player.rb.velocity.y);
        }
        else
        {
            player.Setvelocity(0, player.rb.velocity.y);
        }
    }

    public override void Update()
    {
        if (player.iswalldetected())
        {
            Debug.Log("Wall Dash - Diagonal Jump");

            player.wallDashDirection = -player.facingdirection;
            player.isWallDashing = true;

            player.Flip();
            player.Setvelocity(player.facingdirection*wallDashHorizontalSpeed*30f,player.rb.velocity.y);
            statemachine.changestate(player.airstate);
            return;
        }
        if (player.isWallDashing)
        {
            float progress = 1 - (stateTimer / player.wallDashDuration); // 0 -> 1
            
            float currentVerticalSpeed = Mathf.Lerp(wallDashVerticalSpeed, wallDashVerticalSpeed * 0.3f, progress);
            
            player.rb.velocity = new Vector2(wallDashHorizontalSpeed, currentVerticalSpeed);
        }
        else if (isVerticalDash)
        {
            player.rb.velocity = new Vector2(player.dashspeed * player.dashdirection*10, player.dashspeed*5);
        }
        else
        {
            // 普通水平冲刺
            player.Setvelocity(player.dashspeed * player.dashdirection, 0);
        }
        
        // 冲刺结束
        if (stateTimer < 0)
        {
            statemachine.changestate(player.airstate);
        }
        
        base.Update();
    }
}