using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public abstract class Character : MonoBehaviour
{
    public enum BATTLESTATES
    {
        Neutral = 0,
        InFight,
        NbStates
    }

    public enum STATES
    {
        Neutral = 0,
        CantAttack,
        LiftedShield,
        Dead,
        nbStates
    }

    protected float attackSpeed;
    [SerializeField] protected float baseAttackSpeed;

    [SerializeField] protected int baseLife;
    [SerializeField] protected int baseMana;
    [SerializeField] protected float baseRegenLife;
    [SerializeField] protected float baseRegenMana;
    [SerializeField] protected int baseWeight;

    protected BATTLESTATES battleState = BATTLESTATES.Neutral;

    private CharacterAttackZone characAttackZone;
    [SerializeField] protected int constitution;
    protected int curLife;

    protected int curMana;

    protected int curWeight;
    [SerializeField] protected int dexterity;
    [SerializeField] protected int intelligence;

    protected int level = 1;

    protected int maxLife;
    protected int maxMana;

    protected int maxWeight;
    protected float precision;
    protected float regenLife;
    protected float regenMana;
    protected STATES state = STATES.Neutral;

    [SerializeField] protected int strength;

    public STATES State
    {
        get { return state; }
        set { state = value; }
    }

    public BATTLESTATES BattleState
    {
        get { return battleState; }
        set { battleState = value; }
    }

    public Equipment equipment { get; private set; }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public int MaxLife
    {
        get { return maxLife; }
        set { maxLife = value; }
    }

    public virtual int CurLife
    {
        get { return curLife; }
        set { curLife = Mathf.Min(maxLife, value); }
    }

    public int MaxMana
    {
        get { return maxMana; }
        set { maxMana = value; }
    }

    public virtual int CurMana
    {
        get { return curMana; }
        set { curMana = Mathf.Min(maxMana, value); }
    }

    public int Strength
    {
        get { return strength; }
        set { strength = value; }
    }

    public int Dexterity
    {
        get { return dexterity; }
        set { dexterity = value; }
    }

    public int Constitution
    {
        get { return constitution; }
        set { constitution = value; }
    }

    public int Intelligence
    {
        get { return intelligence; }
        set { intelligence = value; }
    }

    public float AttackSpeed
    {
        get { return attackSpeed; }
        set { attackSpeed = value; }
    }

    public float Precision
    {
        get { return precision; }
        set { precision = value; }
    }

    public float RegenLife
    {
        get { return regenLife; }
        set { regenLife = value; }
    }

    public float RegenMana
    {
        get { return regenMana; }
        set { regenMana = value; }
    }

    public int MaxWeight
    {
        get { return maxWeight; }
    }

    public int CurWeight
    {
        get { return curWeight; }
        set { curWeight = Mathf.Min(maxWeight, value); }
    }

    private void Awake()
    {
        equipment = gameObject.GetComponent<Equipment>();
        characAttackZone = transform.Find("AttackZone").GetComponent<CharacterAttackZone>();

        curLife = maxLife;
        curMana = maxMana;

        RecalculateCharacterStats();
    }

    public void Attack()
    {
        StartCoroutine(characAttackZone.ActiveFor(1f / attackSpeed));
    }

    public int MakeDamage()
    {
        float damage = strength;
        float rand = Random.Range(0, 100);

        if (rand <= precision)
            return (int) (damage * 1.25f);

        return (int) damage;
    }

    public virtual void UndergoAttack(int damage, Character charac = null)
    {
        damage = damage - constitution;

        if (state != STATES.LiftedShield && damage > 0)
            damage /= 2;

        CurLife -= damage;
        CheckIsAlive();
    }

    public void RecalculateCharacterStats()
    {
        CalculateLife();
        CalculateMana();
        CalculateMaxWeight();
        CalculateAttackSpeed();
        CalculatePrecision();
        CalculateRegenLife();
        CalculateRegenMana();
    }

    public IEnumerator CouldownAttack()
    {
        state = STATES.CantAttack;
        yield return new WaitForSeconds(1f / attackSpeed);
        state = STATES.Neutral;
    }

    private void CalculateLife()
    {
        int prevMaxLife = maxLife;
        maxLife = baseLife + constitution;

        int addedMaxLife = maxLife - prevMaxLife;
        curLife += addedMaxLife;
    }

    private void CalculateMana()
    {
        int prevMaxMana = maxMana;
        maxMana = baseMana + intelligence;

        int addedMaxMana = maxMana - prevMaxMana;
        curMana += addedMaxMana;
    }

    private void CalculateMaxWeight()
    {
        maxWeight = baseWeight + 5 * strength;
    }

    private void CalculateAttackSpeed()
    {
        attackSpeed = baseAttackSpeed + 0.05f * dexterity;
    }

    private void CalculatePrecision()
    {
        precision = 0.3f * dexterity;
    }

    protected void CalculateRegenLife()
    {
        regenLife = baseRegenLife + 0.5f * constitution;
    }

    protected void CalculateRegenMana()
    {
        regenMana = baseRegenMana + 0.5f * intelligence;
    }

    public virtual void IsDead()
    {
    }

    protected void CheckIsAlive()
    {
        if (curLife <= 0)
        {
            state = STATES.Dead;
            IsDead();
        }
    }
}