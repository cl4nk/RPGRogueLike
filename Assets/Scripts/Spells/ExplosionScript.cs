using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Spells
{
    public class ExplosionScript : MonoBehaviour
    {
        private int damage;
        private float duration;

        private List<GameObject> enemies;

        private bool followPlayer;
        private float frequency;

        private Transform player;

        private float speed;
        private float timeStart;

        public void Init(float speed, float duration, float frequency, int damage, bool follow)
        {
            this.speed = speed;
            this.duration = duration;
            this.frequency = frequency;
            this.damage = damage;
            followPlayer = follow;

            timeStart = Time.time;
            player = FindObjectOfType<Player>().transform;
            enemies = new List<GameObject>();

            StartCoroutine(DamageCoroutine());
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (followPlayer && player)
                transform.position = player.position;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.tag == "Enemy")
                enemies.Add(collider.gameObject);
        }

        private void OnTriggerExit(Collider collider)
        {
            if (enemies.Contains(collider.gameObject))
                enemies.Remove(collider.gameObject);
        }

        private IEnumerator DamageCoroutine()
        {
            while (Time.time - timeStart < duration)
            {
                foreach (GameObject enemy in enemies)
                {
                    Enemy enemyScript = enemy.GetComponent<Enemy>();
                    enemyScript.UndergoAttack(damage, player.GetComponent<Player>());
                }
                yield return new WaitForSeconds(frequency);
            }
            Destroy(gameObject);
        }
    }
}