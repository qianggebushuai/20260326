using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// 背包拖拽管理器
/// 处理物品的拖拽、放置、堆叠、交换
/// </summary>
public class InventoryDragManager : MonoBehaviour
{
    public static InventoryDragManager instance;

    [Header("拖拽图标设置")]
    [SerializeField] private Canvas parentCanvas;           // 父Canvas
    [SerializeField] private GameObject dragIconPrefab;     // 拖拽图标预制体（可选）

    [Header("拖拽图标样式")]
    [SerializeField] private Vector2 dragIconSize = new Vector2(50, 50);
    [SerializeField] private float dragIconAlpha = 0.8f;
    [SerializeField] private Vector2 dragOffset = new Vector2(25, -25);  // 图标偏移（相对鼠标）

    [Header("原槽位样式")]
    [SerializeField] private float originalSlotAlpha = 0.3f;  // 拖拽时原槽位透明度

    // 拖拽状态
    private bool isDragging = false;
    private int dragStartSlotIndex = -1;
    private inventoryitem draggedItem;
    private InventorySlotUI dragStartSlot;

    // 拖拽UI
    private GameObject dragIconObject;
    private Image dragIconImage;
    private TextMeshProUGUI dragStackText;
    private RectTransform dragIconRect;

    // 当前悬停的槽位
    private InventorySlotUI currentHoveredSlot;

    // 公共属性
    public bool IsDragging => isDragging;
    public int DragStartIndex => dragStartSlotIndex;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (parentCanvas == null)
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                parentCanvas = FindObjectOfType<Canvas>();
            }
        }

        CreateDragIcon();
    }

    private void Update()
    {
        if (isDragging)
        {
            UpdateDragIconPosition();

            // 检测鼠标松开
            if (Input.GetMouseButtonUp(0))
            {
                EndDrag();
            }
        }
    }

    #region 拖拽图标创建

    /// <summary>
    /// 创建拖拽图标（预先创建，隐藏状态）
    /// </summary>
    private void CreateDragIcon()
    {
        // 创建拖拽图标对象
        dragIconObject = new GameObject("DragIcon");
        dragIconObject.transform.SetParent(parentCanvas.transform, false);

        // 添加RectTransform
        dragIconRect = dragIconObject.AddComponent<RectTransform>();
        dragIconRect.sizeDelta = dragIconSize;
        dragIconRect.pivot = new Vector2(0.5f, 0.5f);

        // 添加Image组件
        dragIconImage = dragIconObject.AddComponent<Image>();
        dragIconImage.raycastTarget = false; 

        // 添加数量文本
        GameObject textObj = new GameObject("StackText");
        textObj.transform.SetParent(dragIconObject.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(1, 0);
        textRect.anchorMax = new Vector2(1, 0);
        textRect.pivot = new Vector2(1, 0);
        textRect.anchoredPosition = new Vector2(-2, 2);
        textRect.sizeDelta = new Vector2(30, 20);

        dragStackText = textObj.AddComponent<TextMeshProUGUI>();
        dragStackText.fontSize = 14;
        dragStackText.alignment = TextAlignmentOptions.BottomRight;
        dragStackText.raycastTarget = false;

        // 添加CanvasGroup用于控制透明度
        CanvasGroup canvasGroup = dragIconObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = dragIconAlpha;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        // 确保在最上层
        dragIconObject.transform.SetAsLastSibling();

        // 初始隐藏
        dragIconObject.SetActive(false);
    }

    #endregion

    #region 拖拽控制

    /// <summary>
    /// 开始拖拽（由槽位调用）
    /// </summary>
    public void StartDrag(InventorySlotUI slot, int slotIndex, inventoryitem item)
    {
        if (item == null || item.data == null)
        {
            Debug.Log("[DragManager] 无法拖拽空槽位");
            return;
        }

        isDragging = true;
        dragStartSlotIndex = slotIndex;
        draggedItem = item;
        dragStartSlot = slot;

        // 设置拖拽图标
        dragIconImage.sprite = item.data.icon;
        dragIconImage.enabled = true;

        // 设置数量文本
        if (item.stacksize > 1)
        {
            dragStackText.text = item.stacksize.ToString();
            dragStackText.enabled = true;
        }
        else
        {
            dragStackText.enabled = false;
        }

        // 显示拖拽图标
        dragIconObject.SetActive(true);
        dragIconObject.transform.SetAsLastSibling();

        // 更新位置
        UpdateDragIconPosition();

        // 设置原槽位半透明
        slot.SetDragging(true, originalSlotAlpha);

        Debug.Log($"[DragManager] 开始拖拽: {item.data.itemname} 从槽位 {slotIndex}");
    }

    /// <summary>
    /// 更新拖拽图标位置
    /// </summary>
    private void UpdateDragIconPosition()
    {
        if (dragIconRect == null) return;

        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.worldCamera,
            out mousePos
        );

        dragIconRect.anchoredPosition = mousePos + dragOffset;
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    private void EndDrag()
    {
        if (!isDragging) return;

        // 检测当前悬停的槽位
        InventorySlotUI targetSlot = GetSlotUnderMouse();

        if (targetSlot != null && targetSlot != dragStartSlot)
        {
            int targetIndex = targetSlot.GetSlotIndex();

            // 尝试堆叠或交换
            TryStackOrSwap(dragStartSlotIndex, targetIndex);
        }
        else
        {
            // 没有有效目标，取消拖拽
            Debug.Log("[DragManager] 拖拽取消，物品返回原位");
        }

        // 恢复原槽位状态
        if (dragStartSlot != null)
        {
            dragStartSlot.SetDragging(false, 1f);
        }

        // 隐藏拖拽图标
        dragIconObject.SetActive(false);

        // 重置状态
        isDragging = false;
        dragStartSlotIndex = -1;
        draggedItem = null;
        dragStartSlot = null;
        currentHoveredSlot = null;

        // 刷新UI
        inventory.instance?.UpdateAllUI();
    }

    public void CancelDrag()
    {
        if (isDragging)
        {
            // 恢复原槽位状态
            if (dragStartSlot != null)
            {
                dragStartSlot.SetDragging(false, 1f);
            }

            // 隐藏拖拽图标
            dragIconObject.SetActive(false);

            // 重置状态
            isDragging = false;
            dragStartSlotIndex = -1;
            draggedItem = null;
            dragStartSlot = null;

            Debug.Log("[DragManager] 拖拽已取消");
        }
    }

    #endregion

    #region 槽位检测

    /// <summary>
    /// 获取鼠标下方的槽位
    /// </summary>
    private InventorySlotUI GetSlotUnderMouse()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            InventorySlotUI slot = result.gameObject.GetComponent<InventorySlotUI>();
            if (slot == null)
            {
                slot = result.gameObject.GetComponentInParent<InventorySlotUI>();
            }

            if (slot != null)
            {
                return slot;
            }
        }

        return null;
    }

    /// <summary>
    /// 设置当前悬停的槽位（由槽位调用）
    /// </summary>
    public void SetHoveredSlot(InventorySlotUI slot)
    {
        if (isDragging)
        {
            currentHoveredSlot = slot;
        }
    }

    /// <summary>
    /// 清除悬停槽位
    /// </summary>
    public void ClearHoveredSlot(InventorySlotUI slot)
    {
        if (currentHoveredSlot == slot)
        {
            currentHoveredSlot = null;
        }
    }

    #endregion

    #region 堆叠与交换逻辑

    /// <summary>
    /// 尝试堆叠或交换物品
    /// </summary>
    private void TryStackOrSwap(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;

        inventoryitem fromItem = inventory.instance.GetItemAtSlot(fromIndex);
        inventoryitem toItem = inventory.instance.GetItemAtSlot(toIndex);

        // 目标槽位为空 - 直接移动
        if (toItem == null)
        {
            inventory.instance.MoveItem(fromIndex, toIndex);
            Debug.Log($"[DragManager] 移动物品到空槽位: {fromIndex} → {toIndex}");
            return;
        }

        // 相同物品且可堆叠 - 尝试堆叠
        if (fromItem.data == toItem.data && toItem.data.canStack)
        {
            int canStack = toItem.data.maxStackSize - toItem.stacksize;

            if (canStack > 0)
            {
                int toStack = Mathf.Min(canStack, fromItem.stacksize);
                inventory.instance.StackItems(fromIndex, toIndex, toStack);
                Debug.Log($"[DragManager] 堆叠物品: {toStack}个 {fromItem.data.itemname}");
                return;
            }
        }

        // 不能堆叠 - 交换位置
        inventory.instance.SwapSlots(fromIndex, toIndex);
        Debug.Log($"[DragManager] 交换物品: 槽位 {fromIndex} ↔ {toIndex}");
    }

    #endregion
}