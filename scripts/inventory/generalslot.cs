using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 背包槽位 UI（支持拖拽）
/// </summary>
public class InventorySlotUI : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("UI 组件")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image highlightBorder;       // 选中高亮
    [SerializeField] private Image hoverHighlight;        // 悬停高亮
    [SerializeField] private TextMeshProUGUI stackText;   // 堆叠数量
    [SerializeField] private TextMeshProUGUI slotNumberText;  // 快捷键数字

    [Header("拖拽设置")]
    [SerializeField] private float dragThreshold = 5f;    // 拖拽阈值（像素）

    [Header("颜色设置")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.3f);
    [SerializeField] private Color dragTargetColor = new Color(0f, 1f, 0f, 0.3f);

    private int slotIndex = -1;
    private inventoryitem currentItem;
    private bool isSelected = false;

    // 拖拽状态
    private bool isPointerDown = false;
    private Vector2 pointerDownPosition;
    private bool isDragStarted = false;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    #region 初始化与更新

    /// <summary>
    /// 设置槽位索引
    /// </summary>
    public void SetSlotIndex(int index)
    {
        slotIndex = index;

        // 显示快捷键数字（只有快捷栏前9个）
        if (slotNumberText != null)
        {
            if (index < 9)
            {
                slotNumberText.text = (index + 1).ToString();
                slotNumberText.enabled = true;
            }
            else
            {
                slotNumberText.enabled = false;
            }
        }
    }

    /// <summary>
    /// 获取槽位索引
    /// </summary>
    public int GetSlotIndex()
    {
        return slotIndex;
    }

    /// <summary>
    /// 更新槽位显示
    /// </summary>
    public void UpdateSlot(inventoryitem item, bool selected)
    {
        currentItem = item;
        isSelected = selected;

        if (item != null && item.data != null)
        {
            // 显示物品图标
            if (itemIcon != null)
            {
                itemIcon.sprite = item.data.icon;
                itemIcon.enabled = true;
            }

            // 显示堆叠数量
            if (stackText != null)
            {
                if (item.stacksize > 1)
                {
                    stackText.text = item.stacksize.ToString();
                    stackText.enabled = true;
                }
                else
                {
                    stackText.enabled = false;
                }
            }
        }
        else
        {
            // 清空槽位
            if (itemIcon != null)
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            if (stackText != null)
            {
                stackText.enabled = false;
            }
        }

        // 设置选中高亮
        if (highlightBorder != null)
        {
            highlightBorder.enabled = selected;
            highlightBorder.color = selectedColor;
        }
    }

    /// <summary>
    /// 清空槽位
    /// </summary>
    public void CleanUpSlot()
    {
        UpdateSlot(null, false);
    }

    /// <summary>
    /// 设置拖拽状态（调整透明度）
    /// </summary>
    public void SetDragging(bool dragging, float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        else if (itemIcon != null)
        {
            Color c = itemIcon.color;
            c.a = alpha;
            itemIcon.color = c;
        }
    }

    /// <summary>
    /// 设置为拖拽目标高亮
    /// </summary>
    public void SetAsDropTarget(bool isTarget)
    {
        if (hoverHighlight != null)
        {
            hoverHighlight.enabled = isTarget;
            hoverHighlight.color = isTarget ? dragTargetColor : hoverColor;
        }
    }

    #endregion

    #region 指针事件

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        isPointerDown = true;
        pointerDownPosition = eventData.position;
        isDragStarted = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 如果没有开始拖拽，视为点击
        if (isPointerDown && !isDragStarted)
        {
            OnSlotClicked();
        }

        isPointerDown = false;
        isDragStarted = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 悬停高亮
        if (hoverHighlight != null && !InventoryDragManager.instance.IsDragging)
        {
            hoverHighlight.enabled = true;
            hoverHighlight.color = hoverColor;
        }

        // 通知拖拽管理器
        if (InventoryDragManager.instance.IsDragging)
        {
            InventoryDragManager.instance.SetHoveredSlot(this);
            SetAsDropTarget(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 取消悬停高亮
        if (hoverHighlight != null)
        {
            hoverHighlight.enabled = false;
        }

        // 通知拖拽管理器
        if (InventoryDragManager.instance != null)
        {
            InventoryDragManager.instance.ClearHoveredSlot(this);
            SetAsDropTarget(false);
        }
    }

    #endregion

    #region 拖拽事件
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (currentItem == null || currentItem.data == null) return;
        float distance = Vector2.Distance(eventData.position, pointerDownPosition);
        if (distance < dragThreshold) return;
        isDragStarted = true;
        InventoryDragManager.instance?.StartDrag(this, slotIndex, currentItem);
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragStarted = false;
        isPointerDown = false;
    }

    #endregion

    #region 点击处理

    /// <summary>
    /// 槽位被点击（非拖拽）
    /// </summary>
    private void OnSlotClicked()
    {
        // 只有快捷栏可以通过点击选择
        if (slotIndex < 9 && !inventory.instance.IsInventoryOpen())
        {
            inventory.instance?.SelectSlot(slotIndex);
        }
        else if (inventory.instance.IsInventoryOpen())
        {
            Debug.Log($"[Slot] 点击槽位 {slotIndex}");
        }
    }

    #endregion
}