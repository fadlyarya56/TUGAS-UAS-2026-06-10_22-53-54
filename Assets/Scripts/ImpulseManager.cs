using UnityEngine;
using Unity.Cinemachine;

public class CameraImpulseManager : MonoBehaviour
{
    public static CameraImpulseManager Instance;

    public CinemachineImpulseSource impulse;

    void Awake()
    {
        Instance = this;
    }

    public void Shake()
    {
        impulse.GenerateImpulse();
    }
}