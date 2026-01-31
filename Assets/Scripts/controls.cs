using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHorizontalMovement : MonoBehaviour
{
    public float moveSpeed = 6f;

    void Update()
    {
        float move = 0f;

        if (Keyboard.current.aKey.isPressed)
            move -= 1f;

        if (Keyboard.current.dKey.isPressed)
            move += 1f;

        Vector3 velocity = new Vector3(move * moveSpeed, 0f, 0f);
        transform.Translate(velocity * Time.deltaTime);
    }
}
