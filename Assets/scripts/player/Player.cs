
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownPlayer : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    [SerializeField] private float inputDeadZone = 0.1f;
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public SpriteRenderer sr { get; private set; }

    #region 状态机
    public TopDownStateMachine stateMachine { get; private set; }
    public TopDownIdleState idleState { get; private set; }
    public TopDownMoveState moveState { get; private set; }
    #endregion

    #region 方向记录
    private Vector2 animDirection = Vector2.down;
    public FacingDirection facingDirection { get; private set; } = FacingDirection.Down;

    // 当前帧的输入方向（每帧更新）
    public Vector2 currentMoveInput { get; private set; }
    #endregion

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        stateMachine = new TopDownStateMachine();
        idleState = new TopDownIdleState(this, stateMachine, "Idle");
        moveState = new TopDownMoveState(this, stateMachine, "Move");
    }

    protected virtual void Start()
    {
        // 设置初始方向
        SetAnimatorDirection();
        stateMachine.Initialize(idleState);
    }

    protected virtual void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        currentMoveInput = new Vector2(xInput, yInput);

        stateMachine.currentState.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.currentState.FixedUpdate();
    }

    #region 移动方法

    public void SetVelocity(float x, float y)
    {
        rb.velocity = new Vector2(x, y);
    }

    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// 检查是否有有效移动输入
    /// </summary>
    public bool HasMoveInput()
    {
        return Mathf.Abs(currentMoveInput.x) > inputDeadZone ||
               Mathf.Abs(currentMoveInput.y) > inputDeadZone;
    }

    /// <summary>
    /// 获取标准化的移动输入
    /// </summary>
    public Vector2 GetMoveInputNormalized()
    {
        if (!HasMoveInput()) return Vector2.zero;
        return currentMoveInput.normalized;
    }

    #endregion

    #region 方向控制

    public void UpdateFacingDirection()
    {
        if (!HasMoveInput()) return;

        float xInput = currentMoveInput.x;
        float yInput = currentMoveInput.y;

        bool isDiagonal = Mathf.Abs(xInput) > 0.1f && Mathf.Abs(yInput) > 0.1f;

        if (isDiagonal)
        {
            switch (facingDirection)
            {
                case FacingDirection.Up:
                    if (yInput > 0) return;
                    break;

                case FacingDirection.Down:
                    if (yInput < 0) return;
                    break;

                case FacingDirection.Left:
                    if (xInput < 0) return;
                    break;

                case FacingDirection.Right:
                    if (xInput > 0) return;
                    break;
            }
        }

        if (Mathf.Abs(yInput) > Mathf.Abs(xInput))
        {
            if (yInput > 0)
            {
                facingDirection = FacingDirection.Up;
                animDirection = Vector2.up;
            }
            else
            {
                facingDirection = FacingDirection.Down;
                animDirection = Vector2.down;
            }
        }
        else
        {
            if (xInput > 0)
            {
                facingDirection = FacingDirection.Right;
                animDirection = Vector2.right;
            }
            else
            {
                facingDirection = FacingDirection.Left;
                animDirection = Vector2.left;
            }
        }
    }
    public void SetAnimatorDirection()
    {
        if (anim == null) return;

        anim.SetFloat("MoveX", animDirection.x);
        anim.SetFloat("MoveY", animDirection.y);
    }

    /// <summary>
    /// 设置是否在移动
    /// </summary>
    public void SetMoving(bool isMoving)
    {
        if (anim == null) return;
        anim.SetBool("IsMoving", isMoving);
    }

    #endregion

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishedTrigger();
}

public enum FacingDirection
{
    Down = 0,
    Up = 1,
    Left = 2,
    Right = 3
}