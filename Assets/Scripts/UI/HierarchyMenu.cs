using UnityEngine;

public class HierarchyMenu : MonoBehaviour
{
    public bool blockPrevList = true;

    private int level;

    private NavigableMenu[] menus;

    // Use this for initialization
    private void Start()
    {
        menus = transform.GetComponentsInChildren<NavigableMenu>();
        for (int i = 1; i < menus.Length; i++)
            if (menus[i])
            {
                CanvasGroup cvnGrp = menus[i].GetComponent<CanvasGroup>();
                if (cvnGrp)
                    MakeCanvasInvisible(cvnGrp);
            }
    }

    public void OnClick(NavigableMenu menu, int index)
    {
        if (!menu.GetComponent<CanvasGroup>().interactable)
            return;
        int menuIndex = FindMenuIndex(menu);
        if (index < 0) return;

        if (menuIndex != level && !blockPrevList)
            level = menuIndex;
        if (level < menus.Length - 1)
        {
            MakeCanvasTransparent(menus[level].GetComponent<CanvasGroup>());
            MakeCanvasVisible(menus[++level].GetComponent<CanvasGroup>());
        }
        menus[level].UpdateData(index);
    }

    public void OnClickBack()
    {
        if (level == 0)
            return;
        MakeCanvasInvisible(menus[level].GetComponent<CanvasGroup>());
        level--;
        MakeCanvasVisible(menus[level].GetComponent<CanvasGroup>());
    }

    private void MakeCanvasInvisible(CanvasGroup cvnGrp)
    {
        if (!cvnGrp)
            return;
        cvnGrp.alpha = 0;
        cvnGrp.interactable = false;
    }

    private void MakeCanvasTransparent(CanvasGroup cvnGrp)
    {
        if (!cvnGrp)
            return;
        cvnGrp.alpha = 0.7f;
        if (blockPrevList)
        {
            cvnGrp.interactable = false;
            cvnGrp.blocksRaycasts = false;
        }
    }

    private void MakeCanvasVisible(CanvasGroup cvnGrp)
    {
        if (!cvnGrp)
            return;
        cvnGrp.alpha = 1;
        cvnGrp.interactable = true;
        cvnGrp.blocksRaycasts = true;
    }

    private int FindMenuIndex(NavigableMenu menu)
    {
        if (!menu)
            return -1;
        for (int i = 0; i < menus.Length; i++)
            if (menu == menus[i])
                return i;
        return -1;
    }
}