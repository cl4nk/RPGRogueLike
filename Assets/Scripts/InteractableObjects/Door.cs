using UnityEngine;

namespace InteractableObjects
{
    public class Door : InteractableObject
    {
        private readonly float maxAngle = 90;
        private float angle;
        [SerializeField] private float animationSpeed = 10;

        private bool animDoor;
        private bool opened;

        private Vector3 pivot;

        private void Start()
        {
            ResetPivot();
        }

        public void ResetPivot()
        {
            float width = transform.localScale.x;
            pivot = transform.position + transform.right * (width / 2);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (animDoor)
                AnimDoor();
        }

        public override void OnUse()
        {
            opened = !opened;
            animDoor = true;
        }

        private void AnimDoor()
        {
            if (opened)
                OpenDoor();
            else
                CloseDoor();
        }

        private void OpenDoor()
        {
            angle -= Time.fixedDeltaTime * animationSpeed;
            transform.RotateAround(pivot, Vector3.up, -animationSpeed * Time.deltaTime);

            if (angle <= -maxAngle)
                animDoor = false;
        }

        private void CloseDoor()
        {
            angle += Time.fixedDeltaTime * animationSpeed;
            transform.RotateAround(pivot, Vector3.up, animationSpeed * Time.deltaTime);

            if (angle >= 0)
                animDoor = false;
        }
    }
}