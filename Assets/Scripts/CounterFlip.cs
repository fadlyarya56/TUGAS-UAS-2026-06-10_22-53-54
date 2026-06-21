using UnityEngine;

public class CounterParentFlip : MonoBehaviour
{
    public Transform target;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
        if (target == null)
            target = transform.parent;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float sign = Mathf.Sign(target.localScale.x);
        transform.localScale = new Vector3(baseScale.x * sign, baseScale.y, baseScale.z);
    }
}