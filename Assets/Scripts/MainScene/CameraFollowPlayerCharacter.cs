#region

using UnityEngine;

#endregion

public class CameraFollowPlayerCharacter : MonoBehaviour
{
#region Private Variables

    private Transform player;

#endregion

#region Unity events

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        var playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x , 3.2f , -10);
    }

#endregion
}