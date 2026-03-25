using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entity : MonoBehaviour
{
   
    public int knockdir;
   
    #region Anim
    public Animator anim { get; private set; }
    #endregion
    #region basic
    public Rigidbody2D rb { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    public entityFX fx { get; private set; }
    public Charactorstats stats { get; private set; }
    [SerializeField] protected float movespeed;
    public int facingdirection { get; private set; } = 1;
    protected bool facingright = true;

    [Header("Collision info")]
    public Transform Attackcheck;
    public float Attackcheckradios;
    public Transform Attackcheck2;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;

    [Header("Knockback direction")]
    [SerializeField] protected Vector2 knockbackdirection;
    [SerializeField] protected float knockbackduration;
    public bool isknocked;
    #endregion

    [Header("debuff info")]
    public bool canbeignited;
    public bool canbeshocked;
    public bool canbechilled;
    private float defaultspeed;

    public System.Action onflipped;
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
        knockdir = 1;
        defaultspeed = movespeed;
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<entityFX>();
        stats = GetComponent<Charactorstats>();
        cd = GetComponent<CapsuleCollider2D>();
    }
    protected virtual void Update()
    {
      
    }
    public virtual bool isgrounddetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool iswalldetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingdirection, wallCheckDistance, whatIsGround);
    public virtual void freezetime(bool _timefrozen)
    {
        if (_timefrozen)
        {
            movespeed = 0;
            anim.speed = 0;
        }
        else
        {
            movespeed = defaultspeed;
            anim.speed = 1;
        }
    }
    protected virtual IEnumerator freezetimecoroutine(float _seconds)
    {
        freezetime(true);
        yield return new WaitForSeconds(_seconds);
        freezetime(false);
          
    }
    public virtual void freezetimefor(float _duration) => StartCoroutine(freezetimecoroutine(_duration));
    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingdirection, wallCheck.position.y));
        Gizmos.DrawWireSphere(Attackcheck.position,Attackcheckradios);
        Gizmos.DrawLine(Attackcheck.position, Attackcheck2.position);
    }
    public void Flip()
    {
        facingdirection = facingdirection * -1;
        facingright = !facingright;
        transform.Rotate(0, 180, 0);
        if (onflipped != null)
        {
            onflipped();
        }
      
    }
    protected virtual void Flipcontroller(float _x)
    {
        if (_x > 0 && !facingright)
        {
            Flip();
        }
        else if (_x < 0 && facingright)
        {
            Flip();
        }
    }
    
    protected virtual IEnumerator hitknockback()
    {
        isknocked = true;
        rb.velocity = new Vector2(knockbackdirection.x *knockdir, knockbackdirection.y);
        yield return new WaitForSeconds(knockbackduration);
        isknocked = false;
    }
    public void zerovelocity()
    {
        if (isknocked) {
            return;
           }
        rb.velocity = new Vector2(0, 0);
    }
    public void Setvelocity(float _xVelocity, float _yVelocity)
    {
        if (isknocked)
        {
            return;
        }
        rb.velocity = new Vector2(_xVelocity * movespeed, _yVelocity);
        Flipcontroller(_xVelocity);
    }
    public virtual void die()
    {
        
    }
    public virtual void hiteffect()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("hitknockback");
    }
}
