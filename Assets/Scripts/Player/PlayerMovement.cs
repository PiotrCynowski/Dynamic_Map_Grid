using UnityEngine;

namespace Player {
    public class PlayerMovement : MonoBehaviour {
        [SerializeField] float moveSpeed = 5f;
        float horizontalInput, verticalInput;
        Vector3 moveDirection;

        void Update() {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}