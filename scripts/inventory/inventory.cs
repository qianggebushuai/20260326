using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventory : MonoBehaviour
{
    public static inventory instance;

    [Header("初始物品")]
    public List<itemdata> startingItems;

    [Header("背包设置")]
    [SerializeField] private int hotbarSize = 9;       // 快捷栏大小
    [SerializeField] private int inventorySize = 27;   // 背包大小
    public int TotalSize => hotbarSize + inventorySize; // 总大小 = 36

    [Header("UI 面板")]
    [SerializeField] private GameObject inventoryPanel;    // 背包面板（ESC打开）
    [SerializeField] private GameObject hotbarPanel;       // 快捷栏面板（始终可见）

    [Header("槽位父容器")]
    [SerializeField] private Transform hotbarSlotsParent;     // 快捷栏槽位容器
    [SerializeField] private Transform inventorySlotsParent;  // 背包槽位容器

    private inventoryitem[] inventorySlots;

    public int currentSelectedIndex { get; private set; } = -1;

    // 当前装备的物品
    public inventoryitem currentEquippedItem { get; private set; }


    private InventorySlotUI[] hotbarSlotUIs;
    private InventorySlotUI[] inventorySlotUIs;

    private bool isInventoryOpen = false;
    private Transform playerTransform;

    #region Unity 生命周期

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeInventory();
        InitializeUI();
        FindPlayer();
        AddStartingItems();
        CloseInventory();
    }

    private void Update()
    {
        HandleInput();
    }

    #endregion

    #region 初始化

    /// <summary>
    /// 初始化背包数据
    /// </summary>
    private void InitializeInventory()
    {
        inventorySlots = new inventoryitem[TotalSize];

        // 所有槽位初始化为 null
        for (int i = 0; i < TotalSize; i++)
        {
            inventorySlots[i] = null;
        }

        Debug.Log($"[Inventory] 初始化完成: 快捷栏{hotbarSize}格 + 背包{inventorySize}格 = 总共{TotalSize}格");
    }

    /// <summary>
    /// 初始化 UI 槽位
    /// </summary>
    private void InitializeUI()
    {
        // 获取快捷栏 UI 槽位
        if (hotbarSlotsParent != null)
        {
            hotbarSlotUIs = hotbarSlotsParent.GetComponentsInChildren<InventorySlotUI>(true);

            // 为每个槽位设置索引
            for (int i = 0; i < hotbarSlotUIs.Length; i++)
            {
                hotbarSlotUIs[i].SetSlotIndex(i);
            }
        }

        // 获取背包 UI 槽位
        if (inventorySlotsParent != null)
        {
            inventorySlotUIs = inventorySlotsParent.GetComponentsInChildren<InventorySlotUI>(true);

            // 背包槽位索引从 hotbarSize 开始
            for (int i = 0; i < inventorySlotUIs.Length; i++)
            {
                inventorySlotUIs[i].SetSlotIndex(hotbarSize + i);
            }
        }

        Debug.Log($"[Inventory] UI初始化: 快捷栏槽位{hotbarSlotUIs?.Length} 背包槽位{inventorySlotUIs?.Length}");
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void AddStartingItems()
    {
        if (startingItems == null) return;

        foreach (var item in startingItems)
        {
            if (item != null)
            {
                AddItem(item);
            }
        }
    }

    #endregion

    #region 输入处理

    private void HandleInput()
    {
        // ESC / Tab / I 打开/关闭背包
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // 只在背包关闭时允许使用快捷键
        if (!isInventoryOpen)
        {
            HandleHotbarInput();
            HandleScrollWheelInput();
        }

        // Q 键丢弃当前选中物品（可选）
        if (Input.GetKeyDown(KeyCode.Q) && currentSelectedIndex >= 0)
        {
            // DropItem(currentSelectedIndex);
        }
    }

    private void HandleHotbarInput()
    {
        for (int i = 0; i < hotbarSize && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SelectSlot(i);
                break;
            }
        }
    }

    /// <summary>
    /// 鼠标滚轮切换快捷栏
    /// </summary>
    private void HandleScrollWheelInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            // 向上滚动 - 选择上一个
            int newIndex = currentSelectedIndex - 1;
            if (newIndex < 0) newIndex = hotbarSize - 1;
            SelectSlot(newIndex);
        }
        else if (scroll < 0f)
        {
            // 向下滚动 - 选择下一个
            int newIndex = currentSelectedIndex + 1;
            if (newIndex >= hotbarSize) newIndex = 0;
            SelectSlot(newIndex);
        }
    }

    #endregion

    #region 背包开关

    public void ToggleInventory()
    {
        if (isInventoryOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        isInventoryOpen = true;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        // 可选：暂停游戏
        // Time.timeScale = 0f;

        UpdateAllUI();
        Debug.Log("[Inventory] 背包已打开");
    }

    public void CloseInventory()
    {
        isInventoryOpen = false;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        // 可选：恢复游戏
        // Time.timeScale = 1f;

        UpdateAllUI();
        Debug.Log("[Inventory] 背包已关闭");
    }

    public bool IsInventoryOpen() => isInventoryOpen;

    #endregion

    #region 物品操作（核心）


    public bool AddItem(itemdata item, int amount = 1)
    {
        if (item == null) return false;

        int remaining = amount;

        if (item.canStack)
        {
            remaining = TryStackToExisting(item, remaining);
            if (remaining <= 0)
            {
                UpdateAllUI();
                return true;
            }
        }

        remaining = TryAddToEmptySlots(item, remaining, 0, hotbarSize);
        if (remaining <= 0)
        {
            UpdateAllUI();
            return true;
        }

        // 3. 尝试添加到背包空槽位
        remaining = TryAddToEmptySlots(item, remaining, hotbarSize, TotalSize);
        if (remaining <= 0)
        {
            UpdateAllUI();
            return true;
        }

        if (remaining < amount)
        {
            // 部分添加成功
            UpdateAllUI();
            Debug.Log($"[Inventory] 背包空间不足，只添加了 {amount - remaining}/{amount} 个 {item.itemname}");
            return true;
        }

        Debug.Log("[Inventory] 背包已满，无法添加物品");
        return false;
    }

    private int TryStackToExisting(itemdata item, int amount)
    {
        int remaining = amount;

        for (int i = 0; i < TotalSize && remaining > 0; i++)
        {
            if (inventorySlots[i] != null &&
                inventorySlots[i].data == item &&
                inventorySlots[i].CanStack())
            {
                int canAdd = item.maxStackSize - inventorySlots[i].stacksize;
                int toAdd = Mathf.Min(canAdd, remaining);

                inventorySlots[i].Addstack(toAdd);
                remaining -= toAdd;
            }
        }

        return remaining;
    }

    /// <summary>
    /// 尝试添加到空槽位
    /// </summary>
    private int TryAddToEmptySlots(itemdata item, int amount, int startIndex, int endIndex)
    {
        int remaining = amount;

        for (int i = startIndex; i < endIndex && remaining > 0; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventoryitem newItem = new inventoryitem(item);

                if (item.canStack)
                {
                    int toAdd = Mathf.Min(item.maxStackSize, remaining);
                    newItem.stacksize = toAdd;
                    remaining -= toAdd;
                }
                else
                {
                    newItem.stacksize = 1;
                    remaining -= 1;
                }

                inventorySlots[i] = newItem;
            }
        }

        return remaining;
    }

    public bool RemoveItem(itemdata item, int amount = 1)
    {
        if (item == null) return false;

        int remaining = amount;

        // 从后往前移除（优先移除背包中的）
        for (int i = TotalSize - 1; i >= 0 && remaining > 0; i--)
        {
            if (inventorySlots[i] != null && inventorySlots[i].data == item)
            {
                int toRemove = Mathf.Min(inventorySlots[i].stacksize, remaining);
                inventorySlots[i].Removestack(toRemove);
                remaining -= toRemove;

                // 如果数量为0，清空槽位
                if (inventorySlots[i].stacksize <= 0)
                {
                    // 如果是当前装备的物品，先取消装备
                    if (i == currentSelectedIndex)
                    {
                        UnequipCurrentItem();
                    }

                    inventorySlots[i] = null;
                }
            }
        }

        UpdateAllUI();
        return remaining < amount; // 只要移除了一部分就返回 true
    }

    /// <summary>
    /// 移除指定槽位的物品
    /// </summary>
    public bool RemoveItemAtSlot(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= TotalSize) return false;
        if (inventorySlots[slotIndex] == null) return false;

        inventoryitem item = inventorySlots[slotIndex];
        item.Removestack(amount);

        if (item.stacksize <= 0)
        {
            if (slotIndex == currentSelectedIndex)
            {
                UnequipCurrentItem();
            }
            inventorySlots[slotIndex] = null;
        }

        UpdateAllUI();
        return true;
    }

    /// <summary>
    /// 交换两个槽位的物品
    /// </summary>
    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= TotalSize || indexB < 0 || indexB >= TotalSize)
            return;

        if (indexA == indexB) return;

        // 检查是否可以堆叠
        if (inventorySlots[indexA] != null && inventorySlots[indexB] != null)
        {
            if (inventorySlots[indexA].data == inventorySlots[indexB].data &&
                inventorySlots[indexB].CanStack())
            {
                // 尝试堆叠
                int canAdd = inventorySlots[indexB].data.maxStackSize - inventorySlots[indexB].stacksize;
                int toAdd = Mathf.Min(canAdd, inventorySlots[indexA].stacksize);

                inventorySlots[indexB].Addstack(toAdd);
                inventorySlots[indexA].Removestack(toAdd);

                if (inventorySlots[indexA].stacksize <= 0)
                {
                    inventorySlots[indexA] = null;
                }

                UpdateAllUI();
                return;
            }
        }

        // 直接交换
        inventoryitem temp = inventorySlots[indexA];
        inventorySlots[indexA] = inventorySlots[indexB];
        inventorySlots[indexB] = temp;

        // 更新当前选中索引
        if (currentSelectedIndex == indexA)
        {
            currentSelectedIndex = indexB;
        }
        else if (currentSelectedIndex == indexB)
        {
            currentSelectedIndex = indexA;
        }

        UpdateAllUI();
    }

    #endregion

    #region 装备系统

    /// <summary>
    /// 选择/装备指定槽位（仅限快捷栏）
    /// </summary>
    public void SelectSlot(int slotIndex)
    {
        // 只能选择快捷栏的槽位
        if (slotIndex < 0 || slotIndex >= hotbarSize)
        {
            Debug.Log("[Inventory] 只能选择快捷栏的槽位");
            return;
        }

        // 点击同一个槽位 - 取消选择
        if (currentSelectedIndex == slotIndex)
        {
            DeselectSlot();
            return;
        }

        // 取消旧装备效果
        if (currentEquippedItem != null && currentEquippedItem.data != null)
        {
            currentEquippedItem.data.RemoveEquipEffect(playerTransform);
        }

        // 更新选中状态
        currentSelectedIndex = slotIndex;
        currentEquippedItem = inventorySlots[slotIndex]; // 可能为 null

        // 执行新装备效果
        if (currentEquippedItem != null && currentEquippedItem.data != null)
        {
            currentEquippedItem.data.ExecuteEquipEffect(playerTransform);
            Debug.Log($"[Inventory] 装备了 {currentEquippedItem.data.itemname}");
        }
        else
        {
            Debug.Log($"[Inventory] 选择了空槽位 {slotIndex + 1}");
        }

        UpdateAllUI();
    }

    public void DeselectSlot()
    {
        if (currentEquippedItem != null && currentEquippedItem.data != null)
        {
            currentEquippedItem.data.RemoveEquipEffect(playerTransform);
            Debug.Log($"[Inventory] 取消装备 {currentEquippedItem.data.itemname}");
        }

        currentSelectedIndex = -1;
        currentEquippedItem = null;

        UpdateAllUI();
    }
    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= TotalSize) return;
        if (toIndex < 0 || toIndex >= TotalSize) return;
        if (fromIndex == toIndex) return;

        // 处理当前装备状态
        HandleEquipStateOnMove(fromIndex, toIndex);

        // 移动物品
        inventorySlots[toIndex] = inventorySlots[fromIndex];
        inventorySlots[fromIndex] = null;

        UpdateAllUI();
    }

private void HandleEquipStateOnMove(int fromIndex, int toIndex)
{
    if (currentSelectedIndex == fromIndex)
    {
        // 如果移动的是当前装备
        if (toIndex < hotbarSize)
        {
            // 目标在快捷栏，更新选中索引
            currentSelectedIndex = toIndex;
        }
        else
        {
            // 目标在背包，取消装备
            if (currentEquippedItem != null && currentEquippedItem.data != null)
            {
                currentEquippedItem.data.RemoveEquipEffect(playerTransform);
            }
            currentSelectedIndex = -1;
            currentEquippedItem = null;
        }
    }
}
    /// <summary>
    /// 堆叠物品
    /// </summary>
    public void StackItems(int fromIndex, int toIndex, int amount)
    {
        if (fromIndex < 0 || fromIndex >= TotalSize) return;
        if (toIndex < 0 || toIndex >= TotalSize) return;
        if (fromIndex == toIndex) return;

        inventoryitem fromItem = inventorySlots[fromIndex];
        inventoryitem toItem = inventorySlots[toIndex];

        if (fromItem == null || toItem == null) return;
        if (fromItem.data != toItem.data) return;

        // 堆叠
        int actualAmount = Mathf.Min(amount, fromItem.stacksize);
        toItem.Addstack(actualAmount);
        fromItem.Removestack(actualAmount);

        // 如果源物品数量为0，清空槽位
        if (fromItem.stacksize <= 0)
        {
            // 如果是当前装备，取消装备
            if (fromIndex == currentSelectedIndex)
            {
                DeselectSlot();
            }
            inventorySlots[fromIndex] = null;
        }

        UpdateAllUI();
    }

    public void UnequipCurrentItem()
    {
        DeselectSlot();
    }

    public void OnSlotClicked(int slotIndex)
    {
        // 如果背包已打开，可能是拖拽操作，暂不处理选择
        if (isInventoryOpen)
        {
            // 可以在这里实现拖拽逻辑
            Debug.Log($"[Inventory] 点击了槽位 {slotIndex}");
            return;
        }

        // 背包关闭时，只能点击快捷栏
        if (slotIndex < hotbarSize)
        {
            SelectSlot(slotIndex);
        }
    }

    #endregion
    private void HandleEquipStateOnSwap(int indexA, int indexB)
    {
        if (currentSelectedIndex == indexA)
        {
            // 当前装备在A位置
            if (indexB < hotbarSize)
            {
                // B在快捷栏，A移动到B位置
                currentSelectedIndex = indexB;
                // 效果不变，因为还是同一个物品
            }
            else
            {
                // B在背包，取消装备
                if (currentEquippedItem != null && currentEquippedItem.data != null)
                {
                    currentEquippedItem.data.RemoveEquipEffect(playerTransform);
                }
                currentSelectedIndex = -1;
                currentEquippedItem = null;
            }
        }
        else if (currentSelectedIndex == indexB)
        {
            // 当前装备在B位置
            if (indexA < hotbarSize)
            {
                // A在快捷栏，B移动到A位置
                currentSelectedIndex = indexA;
            }
            else
            {
                // A在背包，取消装备
                if (currentEquippedItem != null && currentEquippedItem.data != null)
                {
                    currentEquippedItem.data.RemoveEquipEffect(playerTransform);
                }
                currentSelectedIndex = -1;
                currentEquippedItem = null;
            }
        }
    }
    #region UI 更新

    public void UpdateAllUI()
    {
        UpdateHotbarUI();
        UpdateInventoryUI();
    }

    /// <summary>
    /// 更新快捷栏 UI
    /// </summary>
    private void UpdateHotbarUI()
    {
        if (hotbarSlotUIs == null) return;

        for (int i = 0; i < hotbarSlotUIs.Length; i++)
        {
            if (i < hotbarSize)
            {
                hotbarSlotUIs[i].UpdateSlot(inventorySlots[i], i == currentSelectedIndex);
            }
        }
    }

    /// <summary>
    /// 更新背包 UI
    /// </summary>
    private void UpdateInventoryUI()
    {
        if (inventorySlotUIs == null) return;

        for (int i = 0; i < inventorySlotUIs.Length; i++)
        {
            int slotIndex = hotbarSize + i;
            if (slotIndex < TotalSize)
            {
                inventorySlotUIs[i].UpdateSlot(inventorySlots[slotIndex], false);
            }
        }
    }

    #endregion

    #region 查询方法


    public inventoryitem GetItemAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= TotalSize) return null;
        return inventorySlots[slotIndex];
    }


    public inventoryitem GetCurrentEquippedItem() => currentEquippedItem;

    public int GetCurrentSelectedIndex() => currentSelectedIndex;

    public bool HasItem(itemdata item)
    {
        return GetItemCount(item) > 0;
    }

    /// <summary>
    /// 获取指定物品的总数量
    /// </summary>
    public int GetItemCount(itemdata item)
    {
        int count = 0;
        for (int i = 0; i < TotalSize; i++)
        {
            if (inventorySlots[i] != null && inventorySlots[i].data == item)
            {
                count += inventorySlots[i].stacksize;
            }
        }
        return count;
    }

    /// <summary>
    /// 检查是否有空槽位
    /// </summary>
    public bool HasEmptySlot()
    {
        for (int i = 0; i < TotalSize; i++)
        {
            if (inventorySlots[i] == null) return true;
        }
        return false;
    }

    /// <summary>
    /// 获取第一个空槽位索引
    /// </summary>
    public int GetFirstEmptySlot()
    {
        for (int i = 0; i < TotalSize; i++)
        {
            if (inventorySlots[i] == null) return i;
        }
        return -1;
    }

    #endregion
}