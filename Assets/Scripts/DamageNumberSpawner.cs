using UnityEngine;

public class DamageNumberSpawner : MonoBehaviour
{
    public static DamageNumberSpawner Instance;
    public GameObject damageNumberPrefab;

    void Awake() => Instance = this;

    public void Spawn(Vector3 worldPos, int damage, bool isCrit)
    {
        if (damageNumberPrefab == null) return;

        GameObject obj = Instantiate(damageNumberPrefab, worldPos, Quaternion.identity);
        DamageNumberText text = obj.GetComponent<DamageNumberText>();
        if (text != null)
            text.Setup(damage, isCrit);
    }
}
