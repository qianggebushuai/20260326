using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// 对话管理器
/// 职责：搜寻匹配对话、控制UI显示、驱动对话流程
/// </summary>
public class DialogueManager : MonoBehaviour
{
    #region 单例
    private static DialogueManager _instance;
    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<DialogueManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    #endregion

    #region UI引用

    [Header("=== UI 引用 ===")]
    [SerializeField] private GameObject dialoguePanel;        // 对话面板根物体
    [SerializeField] private Image leftPortraitImage;          // 左侧立绘Image
    [SerializeField] private Image rightPortraitImage;         // 右侧立绘Image
    [SerializeField] private TextMeshProUGUI speakerNameText;  // 说话者名字
    [SerializeField] private TextMeshProUGUI contentText;      // 对话内容
    [SerializeField] private GameObject continueIndicator;     // "点击继续"提示

    [Header("立绘高亮设置")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    #endregion

    #region 对话数据库

    [Header("=== 对话数据库 ===")]
    [SerializeField] private List<Dialogue> allDialogues = new List<Dialogue>();

    #endregion

    #region 打字机效果设置

    [Header("=== 打字机效果 ===")]
    [SerializeField] private float typeSpeed = 0.03f;          // 每个字的间隔
    [SerializeField] private bool enableTypewriter = true;

    #endregion

    #region 运行时状态

    private Dialogue currentDialogue;
    private int currentLineIndex;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private bool skipTyping = false;
    private Coroutine typewriterCoroutine;

    public bool IsDialogueActive => isDialogueActive;

    // 事件回调
    public System.Action OnDialogueStart;
    public System.Action OnDialogueEnd;
    public System.Action<DialogueLine> OnLineDisplayed;

    #endregion

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        // 点击/按键推进对话
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                // 跳过打字效果，直接显示完整文本
                skipTyping = true;
            }
            else
            {
                // 显示下一句
                DisplayNextLine();
            }
        }
    }

    #region 核心 — 根据NPC搜寻并触发对话

    /// <param name="npcId">NPC的唯一标识</param>
    public void StartDialogueWithNPC(string npcId)
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("[DialogueManager] 对话正在进行中，无法开始新对话");
            return;
        }

        // 搜寻匹配的对话
        Dialogue matched = FindBestDialogue(npcId);

        if (matched == null)
        {
            Debug.Log($"[DialogueManager] 未找到NPC '{npcId}' 的匹配对话");
            return;
        }

        StartDialogue(matched);
    }

    /// <summary>
    /// 直接指定对话开始（可用于剧情触发等）
    /// </summary>
    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.lines.Count == 0)
        {
            Debug.LogWarning("[DialogueManager] 对话为空");
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = 0;
        isDialogueActive = true;

        // 显示UI
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        OnDialogueStart?.Invoke();

        // 显示第一句
        DisplayCurrentLine();
    }

    /// <summary>
    /// 从对话数据库中搜寻最佳匹配的对话
    /// 逻辑：筛选同一npcId + 条件满足 + 最高优先级
    /// </summary>
    private Dialogue FindBestDialogue(string npcId)
    {
        Dialogue bestMatch = null;
        int bestPriority = int.MinValue;

        foreach (Dialogue dialogue in allDialogues)
        {
            // 1. 必须属于当前NPC
            if (dialogue.npcId != npcId)
                continue;

            // 2. 检查条件
            if (dialogue.condition != null && !dialogue.condition.IsMet())
                continue;

            // 3. 选择优先级最高的
            if (dialogue.priority > bestPriority)
            {
                bestPriority = dialogue.priority;
                bestMatch = dialogue;
            }
        }

        return bestMatch;
    }

    #endregion

    #region 对话流程控制

    private void DisplayCurrentLine()
    {
        if (currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        // 更新立绘
        UpdatePortraits(line);

        // 更新说话者名字
        if (speakerNameText != null)
            speakerNameText.text = line.speakerName;

        // 更新对话内容（打字机效果）
        if (enableTypewriter)
        {
            if (typewriterCoroutine != null)
                StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypewriterEffect(line.content));
        }
        else
        {
            if (contentText != null)
                contentText.text = line.content;
        }

        // 隐藏继续提示（打字时）
        if (continueIndicator != null)
            continueIndicator.SetActive(false);

        OnLineDisplayed?.Invoke(line);
    }

    private void DisplayNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
        }
        else
        {
            DisplayCurrentLine();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        isTyping = false;

        // 执行对话完成后的操作
        ExecuteDialogueActions();

        // 自动增加对话次数
        if (!string.IsNullOrEmpty(currentDialogue.npcId))
        {
            GameDialogControl.Instance.IncrementTalkCount(currentDialogue.npcId);
        }

        // 隐藏UI
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        OnDialogueEnd?.Invoke();

        currentDialogue = null;
        currentLineIndex = 0;
    }

    #endregion

    #region 立绘更新

    private void UpdatePortraits(DialogueLine line)
    {
        // 左侧立绘
        if (leftPortraitImage != null)
        {
            if (line.leftPortrait != null)
            {
                leftPortraitImage.gameObject.SetActive(true);
                leftPortraitImage.sprite = line.leftPortrait;
                leftPortraitImage.color = (line.activeSide == SpeakerSide.Left)
                    ? activeColor : inactiveColor;
            }
            else
            {
                leftPortraitImage.gameObject.SetActive(false);
            }
        }

        // 右侧立绘
        if (rightPortraitImage != null)
        {
            if (line.rightPortrait != null)
            {
                rightPortraitImage.gameObject.SetActive(true);
                rightPortraitImage.sprite = line.rightPortrait;
                rightPortraitImage.color = (line.activeSide == SpeakerSide.Right)
                    ? activeColor : inactiveColor;
            }
            else
            {
                rightPortraitImage.gameObject.SetActive(false);
            }
        }

        // 旁白模式
        if (line.activeSide == SpeakerSide.Center)
        {
            if (leftPortraitImage != null)
                leftPortraitImage.color = inactiveColor;
            if (rightPortraitImage != null)
                rightPortraitImage.color = inactiveColor;
        }
    }

    #endregion

    #region 打字机效果

    private IEnumerator TypewriterEffect(string fullText)
    {
        isTyping = true;
        skipTyping = false;
        contentText.text = "";

        foreach (char c in fullText)
        {
            if (skipTyping)
            {
                contentText.text = fullText;
                break;
            }

            contentText.text += c;

            float delay = typeSpeed;
            if (c == '。' || c == '！' || c == '？' || c == '.' || c == '!' || c == '?')
                delay *= 3f;
            else if (c == '，' || c == ',' || c == '；' || c == ';')
                delay *= 2f;

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;

        // 显示继续提示
        if (continueIndicator != null)
            continueIndicator.SetActive(true);
    }

    #endregion

    #region 对话完成后的操作执行

    private void ExecuteDialogueActions()
    {
        if (currentDialogue.actionsOnComplete == null) return;

        GameDialogControl gdc = GameDialogControl.Instance;

        foreach (DialogueAction action in currentDialogue.actionsOnComplete)
        {
            switch (action.actionType)
            {
                case DialogueActionType.SetFlag:
                    gdc.SetFlag(action.stringValue);
                    break;

                case DialogueActionType.RemoveFlag:
                    gdc.RemoveFlag(action.stringValue);
                    break;

                case DialogueActionType.AddItem:
                    gdc.AddItem(action.stringValue);
                    break;

                case DialogueActionType.RemoveItem:
                    gdc.RemoveItem(action.stringValue);
                    break;

                case DialogueActionType.IncrementTalkCount:
                    gdc.IncrementTalkCount(action.stringValue);
                    break;

                case DialogueActionType.TriggerEvent:
                    gdc.TriggerCustomEvent(action.stringValue);
                    break;
            }
        }
    }

    #endregion

    #region 工具方法

    /// <summary>
    /// 运行时添加对话到数据库
    /// </summary>
    public void RegisterDialogue(Dialogue dialogue)
    {
        if (!allDialogues.Contains(dialogue))
            allDialogues.Add(dialogue);
    }

    /// <summary>
    /// 强制结束当前对话
    /// </summary>
    public void ForceEndDialogue()
    {
        if (isDialogueActive)
            EndDialogue();
    }

    #endregion
}