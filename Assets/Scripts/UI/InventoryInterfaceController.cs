using UnityEngine;
using UnityEngine.UI;

public class InventoryInterfaceController : MonoBehaviour
{
    private Text characPrevTxt;

    public Inventory inventoryToShow;

    [SerializeField] private GameObject itemPrefab;

    private NavigableMenu listNavMenu;

    private Text namePrevTxt;

    private Image previewImage;

    private State state;
    private Text valuePrevTxt;
    private Text weightPrevTxt;

    // Use this for initialization
    private void Start()
    {
        listNavMenu = transform.Find("SubList").gameObject.GetComponent<NavigableMenu>();
        if (listNavMenu)
            listNavMenu.OnUpdateData += index =>
            {
                CleanList();
                switch (index)
                {
                    case 0:
                        OnAllClick();
                        break;
                    case 1:
                        OnWeaponsClick();
                        break;
                    case 2:
                        OnClothesClick();
                        break;
                    case 3:
                        UIGame.Instance.ToggleInventoryWindow(inventoryToShow);
                        break;
                    default:
                        break;
                }
            };

        namePrevTxt = transform.Find("ItemPreview/InfoContainer/Name").GetComponent<Text>();
        characPrevTxt = transform.Find("ItemPreview/InfoContainer/Charac").GetComponent<Text>();
        weightPrevTxt = transform.Find("ItemPreview/InfoContainer/WeightValue").GetComponent<Text>();
        valuePrevTxt = transform.Find("ItemPreview/InfoContainer/ValueValue").GetComponent<Text>();
        previewImage = transform.Find("ItemPreview/ItemImage").GetComponent<Image>();
    }

    private void OnClothesClick()
    {
        for (int i = 0; i < inventoryToShow.Items.Count; i++)
        {
            ItemData data = inventoryToShow.Items[i];

            if (data.Type == ItemData.TYPE.ARMOR)
            {
                GameObject item = Instantiate(itemPrefab);
                item.transform.SetParent(listNavMenu.transform);
                Text txt = item.transform.Find("Name").GetComponent<Text>();
                txt.text = data.name;
                item.GetComponent<Button>().onClick.AddListener(() => { SetItemPreview(data); });
            }
        }
        listNavMenu.InitListButtons();
    }

    private void OnWeaponsClick()
    {
        for (int i = 0; i < inventoryToShow.Items.Count; i++)
        {
            ItemData data = inventoryToShow.Items[i];

            if (data.Type == ItemData.TYPE.WEAPON)
            {
                GameObject item = Instantiate(itemPrefab);
                item.transform.SetParent(listNavMenu.transform);
                Text txt = item.transform.Find("Name").GetComponent<Text>();
                txt.text = data.name;
                item.GetComponent<Button>().onClick.AddListener(() => { SetItemPreview(data); });
            }
        }
        listNavMenu.InitListButtons();
    }

    private void OnAllClick()
    {
        for (int i = 0; i < inventoryToShow.Items.Count; i++)
        {
            ItemData data = inventoryToShow.Items[i];
            GameObject item = Instantiate(itemPrefab);
            item.transform.SetParent(listNavMenu.transform);
            Text txt = item.transform.Find("Name").GetComponent<Text>();
            txt.text = data.name;
            item.GetComponent<Button>().onClick.AddListener(() => { SetItemPreview(data); });
        }
        listNavMenu.InitListButtons();
    }

    private void CleanList()
    {
        int count = listNavMenu.transform.childCount;
        for (int i = 0; i < count; i++)
            Destroy(listNavMenu.transform.GetChild(i).gameObject);
    }

    private void SetItemPreview(ItemData data)
    {
        namePrevTxt.text = data.name;
        weightPrevTxt.text = data.weight.ToString();
        valuePrevTxt.text = data.value.ToString();
        characPrevTxt.text = data.ToString();
    }

    private void SetStatusBar()
    {
        Text gold = transform.Find("StatusBar/Gold").GetComponent<Text>();
        Text charac = transform.Find("StatusBar/Charac").GetComponent<Text>();
        Slider lifeSlider = transform.Find("StatusBar/LifeSlider").GetComponent<Slider>();
        Player player = FindObjectOfType<Player>();
        if (player != null)
            lifeSlider.value = player.CurLife / player.MaxLife;
    }

    private void OnEnable()
    {
        SetStatusBar();
    }

    private enum State
    {
        All,
        Weapons,
        Clothes,
        None
    }
}