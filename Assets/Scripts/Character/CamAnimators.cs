using UnityEngine;

namespace Character
{
    public class CamAnimators : MonoBehaviour
    {
        private Animation anim;

        private bool left = true;
        private CharacterController playerController;
        private bool right;

        private void Awake()
        {
            playerController = transform.parent.GetComponent<CharacterController>();
            anim = GetComponent<Animation>();
        }

        private void WalkAnimators()
        {
            if (playerController.isGrounded)
                if (transform.parent.GetComponent<PlayerController>().IsMoving)
                    if (left)
                    {
                        if (!anim.isPlaying)
                        {
                            anim.Play("WalkLeft");
                            left = false;
                            right = true;
                        }
                    }
                    else if (right)
                    {
                        if (!anim.isPlaying)
                        {
                            anim.Play("WalkRight");
                            left = true;
                            right = false;
                        }
                    }
        }

        private void FixedUpdate()
        {
            WalkAnimators();
        }
    }
}