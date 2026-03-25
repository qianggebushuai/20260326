using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class enemy : entity
{
    [Header("stunt info")]
    public float stunduration;
    public Vector2 stundirection;
    [Header("health info")]
   
    public bool canbestunned=false;
    [SerializeField] public GameObject counterimage;


    [Header("attack info")]
    public LayerMask whatisPlayer;

    public float attackdistance;
    public float checkdistance;
   public float attackcooldown;
    [HideInInspector] public float lasttimeattack;
    public float battletime;
    public Transform playerposition;
    // Start is called before the first frame update
    public enemystatemachine statemachine { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        statemachine = new enemystatemachine();


    }
    
    // Update is called once per frame
    protected override void Update()
    {
        if (playerposition != null)
        {
            if (playerposition.position.x > transform.position.x)
            {
                knockdir = -1;
            }
            else
            {
                knockdir = 1;
            }
        }
   
        base.Update();
        statemachine.currentstate.Update();


      

    }
   
    public virtual void opencounterattackwindow()
    {
        canbestunned = true;
        counterimage.SetActive(true);
    }
    public virtual void closecounterattackwindow()
    {
        canbestunned = false;
        counterimage.SetActive(false);
    }
    public virtual bool isstunned()
    {
        if (canbestunned)
        {
            closecounterattackwindow();
            return true;
        }
        return false;
    }

    public virtual void animationfinishtrigger() => statemachine.currentstate.animationfinishtrigger();
    public virtual RaycastHit2D isplayerdetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingdirection, checkdistance, whatisPlayer);
    public virtual RaycastHit2D isplayerdetected2() => Physics2D.Raycast(transform.position, Vector2.down, checkdistance, whatisPlayer);
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackdistance*facingdirection, transform.position.y));
        
    }
    public virtual void damageofenemy()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("hitknockback");
    }
   
}
