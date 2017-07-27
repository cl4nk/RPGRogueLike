using UnityEngine;

public class PreviewRoom : MonoBehaviour
{
    private Animator itemAnimator;
    private Transform itemPosition;

    private void Start()
    {
        itemPosition = transform.Find("ItemPosition");
        itemAnimator = itemPosition.GetComponent<Animator>();
    }

    public void UpdatePreviewPanel(ItemData item)
    {
        if (itemPosition.childCount != 0)
            Destroy(itemPosition.GetChild(0).gameObject);

        GameObject itemToShow;
        itemToShow = Instantiate(item.prefab);
        itemToShow.transform.SetParent(itemPosition);
        itemToShow.transform.localPosition = Vector3.zero;

        itemAnimator.Play("ItemRotation");
    }

    public void ResetPreviewPanel()
    {
        if (itemPosition.childCount != 0)
            Destroy(itemPosition.GetChild(0).gameObject);
    }
}