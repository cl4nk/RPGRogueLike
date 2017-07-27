using Character;
using UnityEngine;

namespace Spells
{
    public class SpellProjectile : MonoBehaviour
    {
        private Vector3 direction;

        private RaySpell spell;
        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
            direction = Camera.main.transform.forward;
        }

        public void InitProjectile(RaySpell spell)
        {
            this.spell = spell;
        }

        private void FixedUpdate()
        {
            transform.position += direction * spell.speed;
            if (Vector3.Distance(transform.position, startPos) >= spell.distance)
                OnEndOfLife();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Character.Character target = collision.gameObject.GetComponent<Character.Character>();
            if (target != null)
                target.UndergoAttack(spell.damage, FindObjectOfType<Player>());
            OnEndOfLife();
        }

        private void OnEndOfLife()
        {
            if (spell.explode)
                Explode();
            Destroy(gameObject);
        }


        private void Explode()
        {
            if (!spell.exploPrefab)
                return;

            GameObject e = Instantiate(spell.exploPrefab,
                transform.position,
                transform.rotation);
            ExplosionScript script = e.GetComponent<ExplosionScript>();
            script.Init(spell.exploSpeed, spell.duration, spell.frequency, spell.damage, false);
        }
    }
}