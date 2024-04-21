using UnityEngine;

namespace Player.Camera {
    public class CameraFollow : MonoBehaviour {
        [SerializeField] Transform target;
        [SerializeField] float smoothSpeed;
        [SerializeField] Vector3 offset;
        Vector3 desiredPosition, smoothedPosition;

        [SerializeField] bool keepTarget = true;

        void LateUpdate() {
            if (keepTarget == true) {
                desiredPosition = target.position + offset;
                smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;

                transform.LookAt(target.transform);
            }
        }
    }
}