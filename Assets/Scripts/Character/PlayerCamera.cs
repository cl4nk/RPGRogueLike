using Managers;
using UI;
using UnityEngine;

namespace Character
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float distance;

        [SerializeField] private float height;

        private float mouseMovementY;

        private float rotationY;

        [SerializeField] private float smooth = 200f;

        private void Start()
        {
            RegisterInputFunctions();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void RegisterInputFunctions()
        {
            InputManager input = InputManager.Instance;

            input.vertical += SetRotationY;

            UIGame.Instance.lockControl += ToggleCursorLock;
            UIGame.Instance.unlockControl += ToggleCursorLock;

            input.menuIsDown += ToggleCursorLock;
            input.characIsDown += ToggleCursorLock;
            input.inventoryIsDown += ToggleCursorLock;
        }

        public void ToggleCursorLock()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void SetRotationY(float axis)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                mouseMovementY = axis;
        }

        private void FixedUpdate()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                float targetHeight = transform.parent.position.y + height;
                float currentHeight = transform.position.y;

                float targetDistance = transform.parent.position.z - distance;
                float currentDistance = transform.position.z;

                currentHeight = Mathf.LerpAngle(currentHeight, targetHeight, smooth * Time.deltaTime);
                currentDistance = Mathf.LerpAngle(currentDistance, targetDistance, smooth * Time.deltaTime);

                transform.position = new Vector3(transform.position.x, currentHeight, currentDistance);

                rotationY += mouseMovementY * smooth * Time.deltaTime;
                rotationY = Mathf.Clamp(rotationY, -60, 60);

                transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
            }
        }
    }
}