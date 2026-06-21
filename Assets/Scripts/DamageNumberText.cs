using UnityEngine;
using UnityEngine.UI;

public class DamageNumberText : MonoBehaviour
{
    public Text label;
    public float floatSpeed = 1.5f;
    public float lifetime = 0.8f;
    public Color normalColor = Color.white;
    public Color critColor = Color.red;

    private float timer;

    public void Setup(int damage, bool isCrit)
    {
        if (isCrit)
        {
            label.text = "-" + damage;
            label.color = critColor;
            transform.localScale *= 1.4f;
        }
        else
        {
            label.text = "-" + damage;
            label.color = normalColor;
        }
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        if (timer > lifetime * 0.5f)
        {
            Color c = label.color;
            c.a = Mathf.Lerp(1f, 0f, (timer - lifetime * 0.5f) / (lifetime * 0.5f));
            label.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
