using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Target")]
    public int slimeTarget = 10;

    [Header("UI")]
    public Text questText; // ganti TMP_Text kalau pakai TextMeshPro

    public int slimesDefeated { get; private set; }
    public int jellyCollected { get; private set; }
    public bool isComplete { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void OnSlimeKilled()
    {
        if (isComplete) return;

        slimesDefeated++;

        if (slimesDefeated >= slimeTarget)
        {
            isComplete = true;
        }

        UpdateUI();
    }

    public void OnJellyCollected()
    {
        jellyCollected++;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (questText == null) return;

        if (isComplete)
        {
            questText.text = "Quest Complete!\nKembali ke Kepala Desa";
        }
        else
        {
            questText.text =
                "Eliminate all slime around the forest\n" +
                "Slime defeated: " + slimesDefeated + "/" + slimeTarget + "\n" +
                "Slime Jelly: " + jellyCollected;
        }
    }
}
