using UnityEngine;

namespace Spells
{
    [CreateAssetMenu(fileName = "RaySpell", menuName = "SpellList/RaySpells", order = 1)]
    public class RaySpell : Spell
    {
        public int damage;
        public float distance;
        public float duration;
        public bool explode;
        public GameObject exploPrefab;
        public float exploSpeed;
        public float frequency;

        public GameObject projectilePrefab;
        public float speed;
    }
}