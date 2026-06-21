using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetHealth(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}