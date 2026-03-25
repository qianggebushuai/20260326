using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("说话者信息")]
    public string speakerName;           // 说话者名字
    public Sprite speakerPortrait;       // 说话者立绘

    [Header("左右两侧立绘（当前这句话的画面状态）")]
    public Sprite leftPortrait;          // 左侧立绘
    public Sprite rightPortrait;         // 右侧立绘

    [Header("谁在说话")]
    public SpeakerSide activeSide;       // 当前说话的是左侧还是右侧

    [Header("对话内容")]
    [TextArea(2, 5)]
    public string content;               // 这句话的文本内容

    [Header("可选：表情/动画触发")]
    public string animationTrigger;      // 可选的动画触发器名称
}

public enum SpeakerSide
{
    Left,
    Right,
    Center  
}