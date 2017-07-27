using UnityEngine;

public class PreviewItem : MonoBehaviour
{
    private Transform itemPosition;

    private void Start()
    {
        itemPosition = transform.Find("ItemPosition");

        // TODO Subscribe an event
    }

    private void ChangeItemPreview(GameObject prefab)
    {
        if (itemPosition.childCount != 0)
            Destroy(itemPosition.GetChild(0).gameObject);

        GameObject item = Instantiate(prefab);
        item.transform.SetParent(itemPosition);
        item.transform.localPosition = Vector3.zero;
    }
}