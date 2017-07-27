using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void SimpleDelegate();

    public bool attachToNavMenu = true;

    public Color color = Color.white;
    private Color defaultColor;
    private Outline outline;

    public void OnDeselect(BaseEventData eventData)
    {
        RemoveEffect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ApplyEffect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveEffect();
    }

    public void OnSelect(BaseEventData eventData)
    {
        ApplyEffect();
    }

    public event SimpleDelegate OnSelected;

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        defaultColor = outline.effectColor;
    }

    private void ApplyEffect()
    {
        if (outline)
            outline.effectColor = color;
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        if (OnSelected != null)
            OnSelected();
    }

    private void RemoveEffect()
    {
        if (outline)
            outline.effectColor = defaultColor;
        transform.localScale = new Vector3(1, 1, 1);
    }
}