using System.Collections;
using UnityEngine;

public class Player : Character
{
    public delegate void PlayerLevelDelegate(int value);

    [SerializeField] private int characPointsOnLevelUp;

    private int experience;

    [SerializeField] private int experienceNextLevel;

    [SerializeField] private int gold;
    private PlayerController playerController;

    private UIGame uiGame;

    private Transform view;

    public override int CurLife
    {
        get { return base.CurLife; }
        set
        {
            base.CurLife = value;
            if (uiGame)
                uiGame.RefreshPlayerInfo();
        }
    }

    public override int CurMana
    {
        get { return base.CurMana; }
        set
        {
            base.CurMana = value;
            if (uiGame)
                uiGame.RefreshPlayerInfo();
        }
    }

    public int CharacPointsOnLevelUp
    {
        get { return characPointsOnLevelUp; }
        set { characPointsOnLevelUp = value; }
    }

    public int Experience
    {
        get { return experience; }
        set
        {
            experience = value;
            HasLevelUp();
            if (uiGame)
                uiGame.RefreshPlayerInfo();
        }
    }

    public int Gold
    {
        get { return gold; }
        set { gold = value; }
    }

    public int ExperienceNextLevel
    {
        get { return experienceNextLevel; }
        set { experienceNextLevel = value; }
    }

    public event PlayerLevelDelegate OnLevelPlayerChange;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        uiGame = UIGame.Instance;
        view = transform.GetChild(0).GetChild(0);
        StartCoroutine(CoolDownRegen());
    }

    private IEnumerator CoolDownRegen()
    {
        while (true)
        {
            if (!playerController.IsMoving && battleState != BATTLESTATES.InFight)
                Regen();
            yield return new WaitForSeconds(2f);
        }
    }

    private void LevelUp()
    {
        experience -= experienceNextLevel;
        experienceNextLevel += experienceNextLevel / 2;
        level++;
    }

    private void HasLevelUp()
    {
        if (experience >= experienceNextLevel)
            LevelUp();
    }

    public bool IsLookingObject(InteractableObject obj)
    {
        RaycastHit hit;

        if (Physics.Raycast(view.position, view.forward, out hit, 20f))
        {
            InteractableObject interactObjectHit = hit.collider.GetComponent<InteractableObject>();

            if (interactObjectHit && interactObjectHit == obj)
                return true;
        }

        return false;
    }

    public void LaunchWeapon(GameObject gameObj)
    {
        GameObject newWeapon = Instantiate(gameObj);
        newWeapon.transform.position = gameObj.transform.position;
        Vector3 dir = view.forward;
        newWeapon.GetComponent<ThrowingWeapon>().Launch(this, dir);
    }

    private void Regen()
    {
        if (state != STATES.Dead)
        {
            if (curLife < maxLife)
                LifeRegen(regenLife);

            if (curMana < maxMana)
                ManaRegen(regenMana);

            UIGame.Instance.RefreshPlayerInfo();
        }
    }

    private void LifeRegen(float value)
    {
        curLife += (int) value;

        if (curLife > maxLife)
            curLife = maxLife;

        CalculateRegenLife();
    }

    private void ManaRegen(float value)
    {
        curMana += (int) value;

        if (curMana > maxMana)
            curMana = maxMana;

        CalculateRegenMana();
    }

    public override void IsDead()
    {
        state = STATES.Dead;
        LevelManager.Instance.GameIsOver("Lose");
    }
}