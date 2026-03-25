using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playeranimationtriggers : MonoBehaviour
{
    
    private GameObject _player;
    Player1 player => GetComponentInParent<Player1>();
    // Start is called before the first frame update
  private void animationtrigger()
    {
        player.Animationtrigger();
    }
 
    private void attacktrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.Attackcheck.position, player.Attackcheckradios);
        foreach(var hit in colliders)
        {


            if (hit.GetComponent<enemy>() != null)
            {

                hit.GetComponent<enemy>().damageofenemy();
                Enemystats _target = hit.GetComponent<Enemystats>();


                if (player.isdashing)
                {
                    player.stats.damage.Addmodifier(-40);

                    player.stats.dodamage(_target);
                    player.stats.damage.Removemodifier(-40);

                }
                else
                {
                    player.stats.dodamage(_target);

                }
               ;

               
                


               // if (hit.GetComponent<enemy>().canbeignited)
                //{
                    //player.stats.onfire(_target);
                    //player.stats.onshock(_target);
               // }




            }
        }
    }

    private void finishtrigger()
    {
        player.Setvelocity(0, 10);
        player.anim.speed = 0;
        player.cd.enabled = false;
        
        
        
    }
   
}
