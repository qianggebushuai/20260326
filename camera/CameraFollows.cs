using UnityEngine;

/// <summary>
/// 增强版相机跟随（已修复抖动问题）
/// </summary>
public class CameraFollowAdvanced : MonoBehaviour
{
    private Camera cam;

    [Header("跟随目标")]
    [SerializeField] private string targetTag = "Player";
    private Transform target;
    private Rigidbody2D targetRb;  // 新增：目标的Rigidbody2D

    [Header("基础设置")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float cameraSize = 10;
    [SerializeField] private float smoothSpeed = 5f;

    [Header("死区设置")]
    [SerializeField] private bool useDeadZone = false;
    [SerializeField] private float deadZoneX = 1f;
    [SerializeField] private float deadZoneY = 1f;

    [Header("边界限制")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-50, -50);
    [SerializeField] private Vector2 maxBounds = new Vector2(50, 50);

    [Header("相机震动")]
    private Vector3 shakeOffset = Vector3.zero;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;

    private void Start()
    {
        FindTarget();
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = cameraSize;
        }

        // 立即对齐到目标位置，避免开场抖动
        if (target != null)
        {
            SnapToTarget();
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        FollowTarget();
    }

    /// <summary>
    /// 跟随目标（修复后的版本）
    /// </summary>
    private void FollowTarget()
    {
        // 计算目标位置
        Vector3 targetPosition = CalculateTargetPosition();

        // 方法1：使用 Lerp（更稳定，推荐）
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        // 方法2：如果仍然抖动，使用 MoveTowards（完全无抖动但不够平滑）
        // Vector3 smoothedPosition = Vector3.MoveTowards(
        //     transform.position, 
        //     targetPosition, 
        //     smoothSpeed * Time.deltaTime
        // );

        if (useBounds)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minBounds.y, maxBounds.y);
        }

        // 应用震动
        UpdateShake();

        // 保持Z轴深度
        smoothedPosition.z = offset.z;

        transform.position = smoothedPosition + shakeOffset;
    }

    /// <summary>
    /// 计算目标位置（考虑死区）
    /// </summary>
    private Vector3 CalculateTargetPosition()
    {
        // 使用目标的实际渲染位置（如果有Rigidbody2D插值）
        Vector3 targetPos;

        if (targetRb != null && targetRb.interpolation != RigidbodyInterpolation2D.None)
        {
            // 使用Rigidbody2D的插值位置
            targetPos = new Vector3(targetRb.position.x, targetRb.position.y, target.position.z);
        }
        else
        {
            targetPos = target.position;
        }

        targetPos += offset;

        if (!useDeadZone)
        {
            return targetPos;
        }

        // 计算相机到目标的距离
        Vector3 delta = targetPos - transform.position;
        delta.z = 0;

        // 死区检测
        if (Mathf.Abs(delta.x) < deadZoneX)
            targetPos.x = transform.position.x;

        if (Mathf.Abs(delta.y) < deadZoneY)
            targetPos.y = transform.position.y;

        return targetPos;
    }

    /// <summary>
    /// 查找目标
    /// </summary>
    private void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);

        if (player != null)
        {
            target = player.transform;
            targetRb = player.GetComponent<Rigidbody2D>();  // 获取Rigidbody2D
            Debug.Log($"[CameraFollow] 找到目标: {player.name}");
        }
        else
        {
            Debug.LogWarning($"[CameraFollow] 未找到Tag为'{targetTag}'的对象");
        }
    }

    #region 相机震动

    public void Shake(float duration, float magnitude)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }

    private void UpdateShake()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            shakeOffset = (Vector3)(Random.insideUnitCircle * shakeMagnitude);
            shakeOffset.z = 0;
            shakeMagnitude *= 0.9f;  // 衰减
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    #endregion

    #region 公共方法

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetRb = newTarget?.GetComponent<Rigidbody2D>();
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;

        if (useBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minBounds.x, maxBounds.x);
            targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);
        }

        targetPos.z = offset.z;
        transform.position = targetPos;
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }

    public void DisableBounds()
    {
        useBounds = false;
    }

    #endregion

    #region 调试绘制

    private void OnDrawGizmosSelected()
    {
        if (useDeadZone)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneX * 2, deadZoneY * 2, 0));
        }

        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, 0);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }

    #endregion
}