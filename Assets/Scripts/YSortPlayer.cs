using UnityEngine;

public class YSortPlayer : MonoBehaviour
{
    [System.Serializable]
    public class SpriteLayer
    {
        public SpriteRenderer sr;
        public int offset;
    }

    public SpriteLayer[] layers;

    void Update()
    {
        int yOrder = Mathf.RoundToInt(-transform.position.y * 100);
        foreach (SpriteLayer layer in layers)
        {
            if (layer.sr != null)
                layer.sr.sortingOrder = yOrder + layer.offset;
        }
    }
}
