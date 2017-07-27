using UnityEngine;

public class Enemy : Character
{
    public delegate void DeathDelegate();

    private ParticleSystem deadParticleSytem;

    private ParticleSystem hurtParticleSystem;
    [SerializeField] private int lootGold = 100;
    [SerializeField] private int lootXp = 10;
    public event DeathDelegate OnDead;


    private void Start()
    {
        hurtParticleSystem = GetComponent<ParticleSystem>();
        deadParticleSytem = transform.Find("DeadParticleEffect").GetComponent<ParticleSystem>();
    }

    public override void IsDead()
    {
        deadParticleSytem.Play(false);

        if (OnDead != null)
            OnDead();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.Experience += lootXp;
        player.Gold += lootGold;
        GetComponent<PathEnemy>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        GetComponent<LootEnemy>().enabled = true;
        GetComponent<SphereCollider>().enabled = true;
        LevelManager.Instance.EnemyLostPlayer(gameObject);
    }

    public override void UndergoAttack(int damage, Character charac)
    {
        if (state != STATES.Dead)
        {
            hurtParticleSystem.Play(false);

            base.UndergoAttack(damage, charac);

            if (!GetComponent<PathEnemy>().Target)
                GetComponent<PathEnemy>().AttackUndergoByPlayer(charac.transform);
        }
    }
}