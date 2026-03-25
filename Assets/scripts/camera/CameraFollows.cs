using UnityEngine;

/// <summary>
/// 使用 FixedUpdate 跟随（与物理同步）
/// </summary>
public class CameraFollowFixed : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private string targetTag = "Player";
    private Transform target;

    [Header("基础设置")]
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float cameraSize = 10;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("边界限制")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Camera cam;
    private Vector3 currentVelocity;

    private void Start()
    {
        FindTarget();
        cam = GetComponent<Camera>();
        if (cam != null)
            cam.orthographicSize = cameraSize;

        if (target != null)
            SnapToTarget();
    }

    /// <summary>
    /// 使用 FixedUpdate 与物理帧同步
    /// </summary>
    private void FixedUpdate()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        Vector3 targetPos = target.position + offset;

        // 使用 SmoothDamp 平滑跟随
        Vector3 newPos = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            1f / smoothSpeed,
            Mathf.Infinity,
            Time.fixedDeltaTime  // 使用固定时间步长
        );

        // 边界限制
        if (useBounds)
        {
            newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
            newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        }

        newPos.z = offset.z;
        transform.position = newPos;
    }

    private void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);
        if (player != null)
            target = player.transform;
    }

    public void SnapToTarget()
    {
        if (target == null) return;
        Vector3 pos = target.position + offset;
        if (useBounds)
        {
            pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
            pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        }
        pos.z = offset.z;
        transform.position = pos;
        currentVelocity = Vector3.zero;
    }

    public void SetBounds(Vector2 min, Vector2 max)
    {
        minBounds = min;
        maxBounds = max;
        useBounds = true;
    }
}