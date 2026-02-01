using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
#region Unity events

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

#endregion
}