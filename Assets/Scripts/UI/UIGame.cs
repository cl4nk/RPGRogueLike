using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    private static UIGame instance;
    private GameObject characWindow;

    private GameObject crosshair;

    private GameObject inGameMenu;

    private InputManager inputManager;
    private RectTransform inventoryWindow;

    private GameObject loadScreen;
    public Player player;
    public PlayerCamera playerCam;
    private PlayerInfo playerInfo;
    private GameObject quitMenu;
    private GameObject resultScreen;
    private SpellsBar spellsBar;
    private GameObject spellsWindow;

    public Compass Compass { get; private set; }

    public static UIGame Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<UIGame>();

            return instance;
        }
    }

    public event Action lockControl;
    public event Action unlockControl;

    private void Awake()
    {
        inputManager = InputManager.Instance;

        playerCam = player.GetComponentInChildren<PlayerCamera>();

        inventoryWindow = transform.Find("InventoryPanel").GetComponent<RectTransform>();
        spellsWindow = transform.Find("SpellsUI").gameObject;
        inGameMenu = transform.Find("InGameMenu").gameObject;
        playerInfo = transform.Find("PlayerInfo").GetComponent<PlayerInfo>();
        spellsBar = transform.Find("SpellsBar").GetComponent<SpellsBar>();
        characWindow = transform.Find("Characteristics").gameObject;
        quitMenu = transform.Find("QuitMenu").gameObject;
        resultScreen = transform.Find("ResultScreen").gameObject;

        crosshair = transform.Find("Crosshair").gameObject;
        Compass = transform.Find("Compass").GetComponent<Compass>();

        AddFunctionsToInputManager();

        ToggleCharacWindow();
    }

    private void Start()
    {
        RefreshPlayerInfo();

        LevelManager.Instance.OnLoadingStart += ShowLoadingScreen;
        LevelManager.Instance.OnLoadingFinish += HideLoadingScreen;
    }

    public void Init(Player _player)
    {
        player = _player;
    }

    public void RefreshPlayerInfo()
    {
        playerInfo.RefreshLife(player.CurLife, player.MaxLife);
        playerInfo.RefreshMana(player.CurMana, player.MaxMana);
        playerInfo.RefreshExp(player.Experience, player.ExperienceNextLevel);
    }

    public void PlayerCantCarry()
    {
        crosshair.GetComponent<Crosshair>().State = Crosshair.ACTION.TooHeavy;
    }

    public void SpellShortcutChanged(Spell spell, int newIndex)
    {
        spellsBar.ChangeSpellSlot(spell, newIndex);
    }

    public void AddPointToCompass(GameObject obj)
    {
        Compass.AddPoint(obj);
    }

    public void RemovePointFromCompass(GameObject obj)
    {
        Compass.RemovePoint(obj);
    }

    public bool EnemyPointOnCompass()
    {
        return Compass.EnemyPointOnCompass();
    }

    public void ToggleCrosshair()
    {
        crosshair.SetActive(!crosshair.gameObject.activeSelf);
    }

    public void ToggleCompass()
    {
        Compass.gameObject.SetActive(!Compass.gameObject.activeSelf);
    }

    public void ToggleInGameMenu()
    {
        inGameMenu.SetActive(!inGameMenu.gameObject.activeSelf);
    }

    public void TogglePlayerInfo()
    {
        playerInfo.gameObject.SetActive(!playerInfo.gameObject.activeSelf);
    }

    public void ToggleCharacWindow()
    {
        HideInGameMenu();
        characWindow.gameObject.SetActive(!characWindow.gameObject.activeSelf);
    }

    public void ToggleSpellWindow()
    {
        HideInGameMenu();
        spellsWindow.gameObject.SetActive(!spellsWindow.gameObject.activeSelf);
    }

    public void ToggleQuitMenu()
    {
        HideInGameMenu();
        quitMenu.SetActive(!quitMenu.gameObject.activeSelf);
    }

    public void ToggleInventoryWindow(Inventory inventory)
    {
        if (!inventoryWindow.gameObject.activeSelf)
        {
            inventoryWindow.GetComponent<InventoryPanel>().ShowInventory(inventory);
        }
        else
        {
            inventoryWindow.GetComponent<InventoryPanel>().HideInventory();
            inventoryWindow.GetComponent<InventoryPanel>().ResetState();
        }
    }

    public void ShowPlayerInventory()
    {
        HideInGameMenu();
        inputManager.LockInventory();
        ToggleInventoryWindow(GameManager.Instance.PlayerInstance.Inventory);
    }

    public void ShowResultScreen(string result)
    {
        resultScreen.SetActive(true);
        resultScreen.transform.Find(result).GetComponent<Text>().enabled = true;
        playerCam.ToggleCursorLock();
        inputManager.LockAllInputs();
    }

    public void LockCharacWindow()
    {
        InputManager inputMgr = InputManager.Instance;
        inputMgr.characIsDown -= ToggleCharacWindow;
        inputMgr.characIsDown -= ToggleCrosshair;
        inputMgr.characIsDown -= playerCam.ToggleCursorLock;

        inputManager.LockAllInputs();
    }

    public void UnlockCharacWindow()
    {
        InputManager inputMgr = InputManager.Instance;
        inputMgr.characIsDown += ToggleCharacWindow;
        inputMgr.characIsDown += ToggleCrosshair;
        inputMgr.characIsDown += playerCam.ToggleCursorLock;

        inputManager.UnlockKey();
    }

    public void UpdateFeedBackPotion(string labelName, float time)
    {
        transform.Find("AlterationPanel/" + labelName).GetComponent<PotionSprite>().ShowPotion(time);
    }

    public void UpdateFeedBackNumberPotion(string labelName, int nbAlteration)
    {
        transform.Find("AlterationPanel/" + labelName).GetComponent<PotionSprite>().UpdateNumberPotion(nbAlteration);
    }

    private void AddFunctionsToInputManager()
    {
        InputManager inputMgr = InputManager.Instance;

        inputMgr.menuIsDown += TogglePlayerInfo;
        inputMgr.menuIsDown += ToggleCrosshair;
        inputMgr.menuIsDown += ToggleInGameMenu;

        lockControl += ToggleCrosshair;
        lockControl += TogglePlayerInfo;

        unlockControl += ToggleCrosshair;
        unlockControl += TogglePlayerInfo;

        inputMgr.characIsDown += ToggleCharacWindow;
        inputMgr.characIsDown += ToggleCrosshair;

        inputMgr.selecSpellIsDown += spellsBar.SelectSpell;
    }

    private void HideInGameMenu()
    {
        if (inGameMenu.gameObject.activeSelf)
            ToggleInGameMenu();
    }

    public void ShowLoadingScreen()
    {
        if (loadScreen)
            return;
        loadScreen = Instantiate(Resources.Load<GameObject>("Prefabs/LoadingScreen"));
        loadScreen.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
    }

    public void HideLoadingScreen()
    {
        if (loadScreen)
            Destroy(loadScreen);
        loadScreen = null;
    }

    private void RaiseLockControl()
    {
        if (lockControl != null) lockControl();
    }

    private void RaiseUnlockControl()
    {
        if (unlockControl != null) unlockControl();
    }
}