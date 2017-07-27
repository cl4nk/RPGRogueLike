using Managers;
using UnityEngine;

namespace UI
{
    public class InGameMenu : MonoBehaviour
    {
        private InputManager inputMgr;
        private UIGame uiGame;

        private void Awake()
        {
            uiGame = transform.parent.GetComponent<UIGame>();
            inputMgr = InputManager.Instance;
        }

        public void CharacPressed()
        {
            uiGame.TogglePlayerInfo();
            uiGame.ToggleCharacWindow();
            inputMgr.LockCharacMenu();
        }

        public void MagicPressed()
        {
            uiGame.ToggleSpellWindow();
            inputMgr.LockAllInputs();
        }

        public void QuitPressed()
        {
            uiGame.ToggleQuitMenu();
            inputMgr.LockAllInputs();
        }

        public void InventoryPressed()
        {
            uiGame.ShowPlayerInventory();
            uiGame.TogglePlayerInfo();
            uiGame.ToggleCrosshair();
        }
    }
}