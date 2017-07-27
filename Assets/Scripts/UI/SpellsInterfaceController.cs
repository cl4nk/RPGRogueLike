using UnityEngine;
using UnityEngine.UI;

public class SpellsInterfaceController : MonoBehaviour
{
    public enum FILTER
    {
        All,
        Ray,
        Range,
        Bonus,
        None
    }

    private Spell[] allSpells;
    private BonusSpell[] bonusSpells;
    private Text costManaTxt;

    private FILTER currFilter = FILTER.None;
    private Text descPrevTxt;

    private Sprite equippedSprite;
    private InputManager inputMgr;
    public GameObject itemPrefab;

    private GameObject itemPreview;
    private Text namePrevTxt;
    private Image previewImage;
    private RangeSpell[] rangeSpells;
    private RaySpell[] raySpells;

    private Spell selectedSpell;

    private SpellsLauncher spellsLauncher;

    private GameObject subList;

    private UIGame uiGame;

    private void Start()
    {
        allSpells = Resources.LoadAll<Spell>("Spells/");
        rangeSpells = Resources.LoadAll<RangeSpell>("Spells/");
        raySpells = Resources.LoadAll<RaySpell>("Spells/");
        bonusSpells = Resources.LoadAll<BonusSpell>("Spells/");

        equippedSprite = Resources.Load<Sprite>("Sprites/equipped_arrow");
        spellsLauncher = FindObjectOfType<SpellsLauncher>();

        uiGame = UIGame.Instance;
        inputMgr = InputManager.Instance;

        NavigableMenu navMenu = transform.Find("MainMenu/SubList").gameObject.GetComponent<NavigableMenu>();
        subList = navMenu.transform.Find("ListContainer").gameObject;

        if (navMenu)
            navMenu.OnUpdateData += PopulateList;

        itemPreview = transform.Find("MainMenu/PreviewContainer/ItemPreview").gameObject;
        namePrevTxt = itemPreview.transform.Find("InfoContainer/Name").GetComponent<Text>();
        descPrevTxt = itemPreview.transform.Find("InfoContainer/Description").GetComponent<Text>();
        costManaTxt = itemPreview.transform.Find("InfoContainer/CostValue").GetComponent<Text>();
        previewImage = itemPreview.transform.Find("ItemImage").GetComponent<Image>();
        HideShortcutPanel();
        HideItemPreview();
    }

    public void HideShortcutPanel()
    {
        GameObject waitPanel = transform.Find("WaitPanel").gameObject;
        waitPanel.SetActive(false);
        PopulateList((int) currFilter);
    }

    public void SetShortcut(int index)
    {
        spellsLauncher.SetSpell(index, selectedSpell);
        uiGame.SpellShortcutChanged(selectedSpell, index);
        HideShortcutPanel();
    }

    public void QuitSpellsInterface()
    {
        inputMgr.UnlockKey();

        uiGame.TogglePlayerInfo();
        uiGame.ToggleCrosshair();
        uiGame.playerCam.ToggleCursorLock();
        uiGame.ToggleSpellWindow();

        TimeManager.Instance.State = TimeManager.StateGame.Running;
    }

    private void OnBonusSpellsClick()
    {
        currFilter = FILTER.Bonus;
        foreach (Spell s in bonusSpells)
            AddItem(s);
    }

    private void OnRangeSpellsClick()
    {
        currFilter = FILTER.Range;
        foreach (Spell s in rangeSpells)
            AddItem(s);
    }

    private void OnRaySpellsClick()
    {
        currFilter = FILTER.Ray;
        foreach (Spell s in raySpells)
            AddItem(s);
    }

    private void OnAllClick()
    {
        currFilter = FILTER.All;
        foreach (Spell s in allSpells)
            AddItem(s);
    }

    private void CleanList()
    {
        int count = subList.transform.childCount;
        for (int i = 0; i < count; i++)
            Destroy(subList.transform.GetChild(i).gameObject);
    }

    private void AddItem(Spell s)
    {
        GameObject item = Instantiate(itemPrefab);
        item.transform.SetParent(subList.transform);

        Text txt = item.transform.Find("Name").GetComponent<Text>();
        txt.text = s.name;
        item.GetComponent<Button>().onClick.AddListener(() =>
        {
            selectedSpell = s;
            ShowShortcutPanel();
        });

        Text equipText = item.transform.Find("EquippedText").GetComponent<Text>();

        if (spellsLauncher.IsSpellAlreadySet(s))
            equipText.text = (spellsLauncher.GetSpellShortcut(s) + 1).ToString();
        else
            equipText.text = "";

        MenuButton menuBtn = item.GetComponentInChildren<MenuButton>();
        menuBtn.OnSelected += () => { SetItemPreview(s); };
    }

    private void SetItemPreview(Spell s)
    {
        ShowItemPreview();
        namePrevTxt.text = s.name;
        descPrevTxt.text = s.description;
        costManaTxt.text = s.costMana.ToString();
        if (!s.sprite)
        {
            previewImage.color = Color.clear;
        }
        else
        {
            previewImage.color = Color.white;
            previewImage.sprite = s.sprite;
        }
    }

    private void ShowShortcutPanel()
    {
        GameObject waitPanel = transform.Find("WaitPanel").gameObject;
        waitPanel.SetActive(true);
    }

    private void ShowItemPreview()
    {
        itemPreview.SetActive(true);
    }

    private void HideItemPreview()
    {
        itemPreview.SetActive(false);
    }

    private void PopulateList(int index)
    {
        CleanList();
        HideItemPreview();
        switch (index)
        {
            case (int) FILTER.All:
                OnAllClick();
                break;
            case (int) FILTER.Ray:
                OnRaySpellsClick();
                break;
            case (int) FILTER.Range:
                OnRangeSpellsClick();
                break;
            case (int) FILTER.Bonus:
                OnBonusSpellsClick();
                break;
            default:
                break;
        }
    }
}