using UnityEngine;

public class YSort : MonoBehaviour
{
    private SpriteRenderer[] sprites;
    public int offset = 2;

    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        int yOrder = Mathf.RoundToInt(-transform.position.y * 100) + offset;
        foreach (SpriteRenderer sr in sprites)
        {
            sr.sortingOrder = yOrder;
        }
    }
}