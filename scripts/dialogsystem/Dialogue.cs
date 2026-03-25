using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Header("对话基本信息")]
    public string dialogueId;       
    public string description;

    [Header("关联NPC")]
    public string npcId;             

    [Header("优先级")]
    public int priority = 0;

    [Header("触发条件")]
    public DialogueCondition condition;

    [Header("对话完成后执行的操作")]
    public List<DialogueAction> actionsOnComplete = new List<DialogueAction>();

    [Header("对话内容 ")]
    public List<DialogueLine> lines = new List<DialogueLine>();
}

/// <summary>
/// 对话完成后可触发的操作
/// </summary>
[System.Serializable]
public class DialogueAction
{
    public DialogueActionType actionType;

    [Header("字符串参数（用于 Flag、NPC ID、事件名称）")]
    public string stringValue;

    [Header("整数参数")]
    public int intValue;

    [Header("★ 物品参数（用于 AddItem / RemoveItem）")]
    public itemdata itemData;

    public int itemAmount = 1;
}

public enum DialogueActionType
{
    SetFlag,            
    RemoveFlag,         
    AddItem,           
    RemoveItem,         
    IncrementTalkCount, 
    TriggerEvent    
}