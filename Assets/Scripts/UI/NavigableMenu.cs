using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class NavigableMenu : MonoBehaviour
    {
        public delegate void IntDelegate(int index);

        private List<Button> buttons;
        private ControllerState crtState = ControllerState.Mouse;
        private GraphicRaycaster graphicRaycaster;

        private HierarchyMenu hMenu;
        public IntDelegate OnUpdateData;
        private int selector;

        private void Start()
        {
            hMenu = GetComponentInParent<HierarchyMenu>();
            GameObject uiGame = GameObject.Find("UIGame");
            GameObject canvas = GameObject.Find("Canvas");
            graphicRaycaster = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();

            InitListButtons();
        }

        public void InitListButtons()
        {
            buttons = new List<Button>();
            Button[] tmpButtons = gameObject.GetComponentsInChildren<Button>();
            int index = 0;

            foreach (Button b in tmpButtons)
                if (b != null)
                    if (b.GetComponent<EventTrigger>() != null)
                    {
                        buttons.Add(b);
                        EventTrigger trigger = b.GetComponent<EventTrigger>();

                        int tempInt = index;

                        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
                        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
                        pointerEnterEntry.callback.AddListener(data => { SetSelector(tempInt); });
                        trigger.triggers.Add(pointerEnterEntry);

                        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
                        pointerExitEntry.eventID = EventTriggerType.PointerExit;
                        pointerExitEntry.callback.AddListener(data => { SetSelector(0); });
                        trigger.triggers.Add(pointerExitEntry);

                        MenuButton menuBtn = b.GetComponent<MenuButton>();
                        if (!menuBtn || menuBtn && menuBtn.attachToNavMenu)
                            b.onClick.AddListener(() =>
                            {
                                if (hMenu)
                                    hMenu.OnClick(this, tempInt);
                            });

                        index++;
                    }
        }

        public void SetControllerState()
        {
            Event e = Event.current;
            if (e != null)
                if (crtState == ControllerState.Keyboard && e.isMouse || Input.GetAxis("Mouse X") != 0 ||
                    Input.GetAxis("Mouse Y") != 0)
                {
                    crtState = ControllerState.Mouse;
                    Cursor.visible = true;
                    if (graphicRaycaster)
                        graphicRaycaster.enabled = true;
                    EventSystem.current.SetSelectedGameObject(null);
                }
                else if (crtState == ControllerState.Mouse && e.isKey)
                {
                    crtState = ControllerState.Keyboard;
                    Cursor.visible = false;
                    if (graphicRaycaster)
                        graphicRaycaster.enabled = false;
                    if (buttons.Count > 0)
                        buttons[selector].Select();
                }
        }

        public void SetSelector(int value)
        {
            selector = value;
        }

        private void OnGUI()
        {
            SetControllerState();
        }

        public void UpdateData(int index)
        {
            if (OnUpdateData != null)
                OnUpdateData(index);
        }

        private enum ControllerState
        {
            Mouse = 1,
            Keyboard = 2
        }
    }
}