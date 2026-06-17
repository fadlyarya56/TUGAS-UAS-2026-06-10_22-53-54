using UnityEngine;

public class SlimeAttack : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player terkena serangan");
        }
    }
}