using UnityEngine;

[CreateAssetMenu(fileName = "LightEffect", menuName = "Data/Item Effect/Light Effect")]
public class LightEffect : itemeffect
{
    [Header("光源效果设置")]
    [SerializeField] private string targetTag = "Player";

    private PlayerLight cachedPlayerLight;  // 缓存引用，避免重复查找

    /// <summary>
    /// 执行效果 - 开启玩家光源
    /// </summary>
    public override void excuteeffect(Transform _position)
    {
        PlayerLight playerLight = FindPlayerLight();

        if (playerLight != null)
        {
            playerLight.isOn = true;
            Debug.Log("[LightEffect] 玩家光源已开启");
        }
        else
        {
            Debug.LogWarning("[LightEffect] 找不到 PlayerLight 组件！");
        }
    }

    /// <summary>
    /// 移除效果 - 关闭玩家光源
    /// </summary>
    public override void removeeffect(Transform _position)
    {
        PlayerLight playerLight = FindPlayerLight();

        if (playerLight != null)
        {
            playerLight.isOn = false;
            Debug.Log("[LightEffect] 玩家光源已关闭");
        }
        else
        {
            Debug.LogWarning("[LightEffect] 找不到 PlayerLight 组件！");
        }
    }

    /// <summary>
    /// 查找玩家的光源组件
    /// </summary>
    private PlayerLight FindPlayerLight()
    {
        // 如果已缓存，直接返回
        if (cachedPlayerLight != null)
            return cachedPlayerLight;

        // 查找玩家物体
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);

        if (player == null)
        {
            Debug.LogError($"[LightEffect] 找不到 Tag 为 '{targetTag}' 的物体！");
            return null;
        }

        // 从子物体中获取 PlayerLight 组件
        cachedPlayerLight = player.GetComponentInChildren<PlayerLight>();

        if (cachedPlayerLight == null)
        {
            Debug.LogError($"[LightEffect] 玩家物体 '{player.name}' 的子物体中找不到 PlayerLight 组件！");
        }

        return cachedPlayerLight;
    }

    /// <summary>
    /// 当ScriptableObject被销毁时清除缓存
    /// </summary>
    private void OnDisable()
    {
        cachedPlayerLight = null;
    }
}