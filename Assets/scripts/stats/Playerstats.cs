using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Playerstats : Charactorstats
{
    private Player1 player;
    [SerializeField] private float staytime;
    // Start is called before the first frame update
    protected override void Start()
    {
        shockdur = 0.5f;
        staytime = shockdur;

        base.Start();
        player = GetComponent<Player1>();
        
    }
    
    // Update is called once per frame
    public override void takedamage(int _damage)
    {
      
        base.takedamage(_damage);
       
        
    }
    public override void dodamage(Charactorstats _targetstats)
    {
        base.dodamage(_targetstats);
       
    }
   
}
