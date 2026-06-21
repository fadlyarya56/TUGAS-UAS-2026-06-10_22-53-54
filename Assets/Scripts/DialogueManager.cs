using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public Text dialogueText;     // ganti TMP_Text kalau pakai TextMeshPro
    public Text speakerNameText;

    private string[] currentLines;
    private int lineIndex;

    void Awake()
    {
        Instance = this;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public bool IsOpen => dialoguePanel != null && dialoguePanel.activeSelf;

    public void StartDialogue(string speaker, string[] lines)
    {
        currentLines = lines;
        lineIndex = 0;

        if (speakerNameText != null)
            speakerNameText.text = speaker;

        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        dialogueText.text = currentLines[lineIndex];
    }

    public void NextLine()
    {
        lineIndex++;
        if (lineIndex >= currentLines.Length)
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            ShowLine();
        }
    }

    void Update()
    {
        if (IsOpen && Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }
}
