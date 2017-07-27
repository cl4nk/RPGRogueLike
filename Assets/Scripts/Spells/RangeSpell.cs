using UnityEngine;

namespace Spells
{
    [CreateAssetMenu(fileName = "RangeSpell", menuName = "SpellList/RangeSpells", order = 1)]
    public class RangeSpell : Spell
    {
        public ExplosionScript animPrefab;
        public int damage;
        public float duration;
        public float frequency;

        public int speed;
    }
}