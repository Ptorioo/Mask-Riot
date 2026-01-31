using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class BackgroundTexture : MonoBehaviour
    {
        private Transform camera;
        private float     startPosX;
        private float     startPosY;

        [SerializeField]
        private float followSpeedX = 3f;
        [SerializeField]
        private float followSpeedY = 3f;

        private void Start()
        {
            camera    = Camera.main.transform;
            startPosX = transform.position.x;
            startPosY = transform.position.y;
        }


        private void Update()
        {
            var distanceX = camera.position.x * followSpeedX;
            var distanceY = camera.position.y * followSpeedY;
            transform.position = new Vector3(startPosX + distanceX ,
                                             startPosY + distanceY ,
                                             transform.position.z);
        }

        public void ForceReset()
        {
            var newPosition  = transform.position;
            var positionX = newPosition.x;
            newPosition.x         = 0 - Mathf.Abs(positionX);
            transform.position = newPosition;
        }
    }
}