using UnityEngine;

public class VillageChiefTrigger : MonoBehaviour
{
    [TextArea(2, 4)]
    public string[] introLines;
    [TextArea(2, 4)]
    public string[] completeLines;

    private bool hasTriggeredIntro = false;
    private bool hasShownEnding = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (DialogueManager.Instance == null) return;

        if (QuestManager.Instance != null && QuestManager.Instance.isComplete)
        {
            if (!hasShownEnding)
            {
                hasShownEnding = true;
                DialogueManager.Instance.StartDialogue("Kepala Desa", completeLines);
            }
        }
        else if (!hasTriggeredIntro)
        {
            hasTriggeredIntro = true;
            DialogueManager.Instance.StartDialogue("Kepala Desa", introLines);
        }
    }
}
