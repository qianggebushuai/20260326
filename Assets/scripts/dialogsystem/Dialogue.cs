using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("对话基本信息")]
    public string dialogueId;            // 对话唯一ID
    public string description;     

    [Header("关联NPC")]
    public string npcId;                 // 这段对话属于哪个NPC

    [Header("优先级（数值越高越优先匹配）")]
    public int priority = 0;

    [Header("触发条件")]
    public DialogueCondition condition;

    [Header("对话完成后执行的操作")]
    public List<DialogueAction> actionsOnComplete = new List<DialogueAction>();

    [Header("对话内容 — 按顺序的每一句话")]
    public List<DialogueLine> lines = new List<DialogueLine>();
}

/// <summary>
/// 对话完成后可触发的操作
/// </summary>
[System.Serializable]
public class DialogueAction
{
    public DialogueActionType actionType;
    public string stringValue;
    public int intValue;
}

public enum DialogueActionType
{
    SetFlag,            // 设置全局标记
    RemoveFlag,         // 移除全局标记
    AddItem,            // 给予物品
    RemoveItem,         // 移除物品
    IncrementTalkCount, // 增加对话计数
    TriggerEvent        // 触发自定义事件
}