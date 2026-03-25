using UnityEngine;

/// <summary>
/// 挂在NPC身上，处理玩家交互触发对话
/// </summary>
public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("NPC标识")]
    public string npcId;

    [Header("交互设置")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 2f;

    [Header("交互提示UI")]
    [SerializeField] private GameObject interactPrompt; // "按E对话" 提示

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            TriggerDialogue();
        }
    }

    public void TriggerDialogue()
    {
        if (DialogueManager.Instance.IsDialogueActive)
            return;

        DialogueManager.Instance.StartDialogueWithNPC(npcId);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}