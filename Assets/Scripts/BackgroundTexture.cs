using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class BackgroundTexture : MonoBehaviour
    {
        private Transform camera;
        private float   startPos;

        [SerializeField]
        private float moveSpeed = 3f;

        private void Start()
        {
            camera   = Camera.main.transform;
            startPos = transform.position.x;
        }

        private void Update()
        {
            float distance = camera.position.x * moveSpeed;
            float movement = camera.position.x * (1 - moveSpeed);
            transform.position = new Vector3(startPos + distance , transform.position.y,
                    transform.position.z);

        }
    }
}