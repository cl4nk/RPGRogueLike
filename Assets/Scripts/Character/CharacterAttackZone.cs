using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class CharacterAttackZone : MonoBehaviour
    {
        private readonly List<Character> alreadyTouchedCharacters = new List<Character>();
        private Character charac; // parent of the attack zone

        private void Awake()
        {
            charac = transform.parent.GetComponent<Character>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            int colliderLayer = collider.gameObject.layer;
            Character touchedCharac = collider.GetComponent<Character>();

            if (colliderLayer == LayerMask.NameToLayer("Character") && touchedCharac != charac &&
                !alreadyTouchedCharacters.Contains(touchedCharac))
            {
                touchedCharac.UndergoAttack(charac.MakeDamage(), charac);
                alreadyTouchedCharacters.Add(touchedCharac);
            }
        }

        public IEnumerator ActiveFor(float time)
        {
            gameObject.SetActive(true);
            yield return new WaitForSeconds(time);
            alreadyTouchedCharacters.Clear();
            gameObject.SetActive(false);
        }
    }
}