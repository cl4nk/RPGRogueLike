using Character;
using Managers;
using UnityEngine;

namespace InteractableObjects
{
    public abstract class InteractableObject : MonoBehaviour
    {
        private bool alreadyLook;
        protected InputManager inputMgr;
        private Player player;

        private void Awake()
        {
            inputMgr = InputManager.Instance;
        }

        protected virtual void FixedUpdate()
        {
            if (player)
                if (player.IsLookingObject(this) && !alreadyLook)
                {
                    inputMgr.useIsDown += OnUse;
                    alreadyLook = true;
                }
                else if (!player.IsLookingObject(this) && alreadyLook)
                {
                    inputMgr.useIsDown -= OnUse;
                    alreadyLook = false;
                }
        }

        private void OnTriggerEnter(Collider collider)
        {
            string colliderTag = collider.tag;

            if (colliderTag == "Player")
                player = collider.gameObject.GetComponent<Player>();
        }

        private void OnTriggerExit(Collider collider)
        {
            string colliderTag = collider.tag;

            if (colliderTag == "Player")
            {
                player = null;
                inputMgr.useIsDown -= OnUse;
                alreadyLook = false;
            }
        }

        public abstract void OnUse();

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            float higherScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius * higherScale);
        }
    }
}