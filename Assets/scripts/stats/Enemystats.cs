using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemystats : Charactorstats
{
    private enemy enemy;
    
    private Rigidbody2D rb;
    [Header("level detail")]
    [SerializeField] private int level;
    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier;

    [SerializeField]private float staytime;
    // Start is called before the first frame update
    protected override void Start()
    {
        Modify(maxHP);
        Modify(damage);
        shockdur = 0.5f;
        staytime = shockdur;
        base.Start();
        enemy = GetComponent<enemy>();
        
        rb = GetComponent<Rigidbody2D>();
    }
    private void Modify(Stats _stats)
    {
        for (int i = 1; i < level; i++)
        {
            float modifiers = _stats.getvalue()*percentageModifier;
            _stats.Addmodifier(Mathf.RoundToInt(modifiers));
        }
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
    public override void doshockdamage(Charactorstats _targetstats)
    {
        int shockdamageup=1;
        staytime -= Time.deltaTime;
        if (staytime < 0)
        {
            shockdur = 0.5f;
            if (rb.velocity.x != 0)
            {
                shockdamageup =  (int)rb.velocity.x;
               
            }
            else
            {
                shockdamageup = 1;
            }
            if (shockdamageup < 1)
            {
                shockdamageup = 1;
            }
           
            if (shockdamageup < 0)
            {
                shockdamageup *= -1;
            }
            shockdur = shockdur/shockdamageup;
            staytime = 0.5f;
        }
        base.doshockdamage(_targetstats);
      
    }
}
