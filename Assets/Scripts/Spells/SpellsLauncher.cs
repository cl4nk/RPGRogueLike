using System.Collections;
using Character;
using Managers;
using UnityEngine;

namespace Spells
{
    public class SpellsLauncher : MonoBehaviour
    {
        private readonly Spell[] spells = new Spell[9];
        private bool canLaunchSpell = true;

        public int currSpellIndex;

        private Player player;

        private void Awake()
        {
            SetCurrentSpell(0);
        }

        private void Start()
        {
            player = FindObjectOfType<Player>();
            InputManager.Instance.spellIsUp += UnlockSpellLauncher;
        }

        public void SetSpell(int index, Spell spell)
        {
            if (spell)
            {
                if (spell.shortcut != -1)
                    spells[spell.shortcut] = null;

                if (spells[index])
                    spells[index].shortcut = -1;

                spells[index] = spell;
                spell.shortcut = index;
            }
        }

        public void SetSpell(int index, string path)
        {
            Spell spellInstance = Resources.Load<Spell>(path);
            SetSpell(index, spellInstance);
        }

        public void SetCurrentSpell(int index)
        {
            if (index >= 0 && index < 9)
                currSpellIndex = index;
        }

        public void UseSpell()
        {
            if (currSpellIndex >= 0)
            {
                Spell currSpell = spells[currSpellIndex];
                if (currSpell && player && canLaunchSpell && player.CurMana >= currSpell.costMana)
                {
                    player.CurMana -= currSpell.costMana;
                    SpellEffect(currSpell);
                    LockSpellLauncher();
                }
            }
        }

        public void RangeSpellEffect(RangeSpell spell)
        {
            ExplosionScript explosionScript =
                Instantiate(spell.animPrefab, player.transform.position, player.transform.rotation);
            GameObject gameObj = explosionScript.gameObject;
            ExplosionScript script = gameObj.GetComponent<ExplosionScript>();
            script.Init(spell.speed, spell.duration, spell.frequency, spell.damage, true);
        }

        public void RaySpellEffect(RaySpell spell)
        {
            Vector3 direction = Camera.main.transform.forward;
            Vector3 startPos = Camera.main.transform.position + direction;
            GameObject gameObj = Instantiate(spell.projectilePrefab, startPos, Camera.main.transform.rotation);
            SpellProjectile projectile = gameObj.GetComponent<SpellProjectile>();
            projectile.InitProjectile(spell);
        }

        public bool IsSpellAlreadySet(Spell s)
        {
            if (!s)
                return false;

            foreach (Spell spell in spells)
                if (spell && spell == s)
                    return true;

            return false;
        }

        public int GetSpellShortcut(Spell s)
        {
            if (!s)
                return -1;

            return s.shortcut;
        }

        public string[] GetSpellPaths()
        {
            string[] spellPaths = new string[9];

            for (int i = 0; i < 9; i++)
                if (spells[i])
                    spellPaths[i] = spells[i].path;

            return spellPaths;
        }

        private void SpellEffect(Spell spell)
        {
            if (spell is RangeSpell)
                RangeSpellEffect((RangeSpell) spell);
            else if (spell is BonusSpell)
                BonusSpellEffect((BonusSpell) spell);
            else if (spell is RaySpell)
                RaySpellEffect((RaySpell) spell);
        }

        private void BonusSpellEffect(BonusSpell spell)
        {
            Character.Character charac = FindObjectOfType<Character.Character>();

            if (spell.isPermanent)
                ApplyBonus(charac, spell.bonusType, spell.bonusValue);
            else
                TemporaryBonusEffect(charac, spell);
        }

        private void ApplyBonus(Character.Character c, BonusSpell.BONUS_TYPE bonusType, int bonusValue)
        {
            switch (bonusType)
            {
                case BonusSpell.BONUS_TYPE.Health:
                    c.CurLife += bonusValue;
                    break;
                case BonusSpell.BONUS_TYPE.Mana:
                    c.CurMana += bonusValue;
                    break;
                case BonusSpell.BONUS_TYPE.Strength:
                    c.Strength += bonusValue;
                    c.RecalculateCharacterStats();
                    break;
                case BonusSpell.BONUS_TYPE.Constitution:
                    c.Constitution += bonusValue;
                    c.RecalculateCharacterStats();
                    break;
                case BonusSpell.BONUS_TYPE.AtkSpeed:
                    c.AttackSpeed += bonusValue;
                    break;
                default:
                    break;
            }
        }

        private void RemoveBonus(Character.Character c, BonusSpell.BONUS_TYPE bonusType, int bonusValue)
        {
            switch (bonusType)
            {
                case BonusSpell.BONUS_TYPE.Health:
                    c.CurLife -= bonusValue;
                    break;
                case BonusSpell.BONUS_TYPE.Mana:
                    c.CurMana -= bonusValue;
                    break;
                case BonusSpell.BONUS_TYPE.Strength:
                    c.Strength -= bonusValue;
                    c.RecalculateCharacterStats();
                    break;
                case BonusSpell.BONUS_TYPE.Constitution:
                    c.Constitution -= bonusValue;
                    c.RecalculateCharacterStats();
                    break;
                case BonusSpell.BONUS_TYPE.AtkSpeed:
                    c.AttackSpeed -= bonusValue;
                    break;
                default:
                    break;
            }
        }

        private IEnumerator TemporaryBonusEffect(Character.Character c, BonusSpell spell)
        {
            ApplyBonus(c, spell.bonusType, spell.bonusValue);
            yield return new WaitForSeconds(spell.duration);
            RemoveBonus(c, spell.bonusType, spell.bonusValue);
        }

        private void LockSpellLauncher()
        {
            canLaunchSpell = false;
        }

        private void UnlockSpellLauncher()
        {
            canLaunchSpell = true;
        }
    }
}