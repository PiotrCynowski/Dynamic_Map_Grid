using UnityEngine;

namespace SmartTiles
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed;
        [SerializeField] private Vector3 offset;
        [SerializeField] private bool keepTarget = true;

        private Vector3 desiredPosition, smoothedPosition;

        private void LateUpdate()
        {
            if (keepTarget == true)
            {
                desiredPosition = target.position + offset;
                smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;

                transform.LookAt(target.transform);
            }
        }
    }
}