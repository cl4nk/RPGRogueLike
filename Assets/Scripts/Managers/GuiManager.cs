using UnityEngine;

namespace Managers
{
    public class GuiManager : MonoBehaviour
    {
        public delegate void SimpleDelegate();

        private GameObject gameOverPrefab;

        private GUIEnum guiState;

        //GameObject mainMenuPrefab;
        private GameObject hudPrefab;

        private GameObject pauseMenuPrefab;

        private PauseEnum pauseState;
        private GameObject shopMenuPrefab;
        private ShopMenuEnum shopMenuState;
        public event SimpleDelegate OnMainMenu;
        public event SimpleDelegate OnPauseMenu;
        public event SimpleDelegate OnShopMenu;

        // Use this for initialization
        private void Start()
        {
            //mainMenuPrefab = Resources.Load<GameObject>("Prefabs/MainMenuPrefab");
            hudPrefab = Resources.Load<GameObject>("Prefabs/HUDPrefab");
            shopMenuPrefab = Resources.Load<GameObject>("Prefabs/ShopPrefab");
            pauseMenuPrefab = Resources.Load<GameObject>("Prefabs/PauseMenuPrefab");
            gameOverPrefab = Resources.Load<GameObject>("Prefabs/MainMenuPrefab");
            //Instantiate(mainMenuPrefab).transform.SetParent(GameObject.Find("Canvas").transform);
        }

        private enum GUIEnum
        {
            MainMenu,
            PauseMenu,
            ShopMenu,
            HUD,
            GameOver
        }


        private enum PauseEnum
        {
            None
        }

        private enum ShopMenuEnum
        {
            None
        }
    }
}