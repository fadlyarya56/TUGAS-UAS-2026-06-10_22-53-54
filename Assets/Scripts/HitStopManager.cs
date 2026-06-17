using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    private bool isStopped = false;

    private void Awake()
    {
        Instance = this;
    }

    public void Stop(float duration)
    {
        if (!isStopped)
            StartCoroutine(HitStop(duration));
    }

    IEnumerator HitStop(float duration)
    {
        isStopped = true;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;
        isStopped = false;
    }
}
