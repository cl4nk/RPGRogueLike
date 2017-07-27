using System.Collections;
using Character;
using Item;
using Spells;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Compass = UI.Compass.Compass;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public delegate void OnLoadingDelegate();

        public enum PLACES
        {
            CampFire = 0,
            Dungeon,
            nbStates
        }

        public const int FIRST_LEVEL_ID = 1;

        private static LevelManager instance;
        private GameObject guiMgr;

        [SerializeField] private GameObject guiMgrPrefab;

        private int mSceneId;

        private PLACES place = PLACES.CampFire;

        [SerializeField] private Player player;

        private GameObject playerGao;

        [SerializeField] private GameObject playerPrefab;

        private UIGame uiGame;

        public PLACES Place
        {
            get { return place; }
            set
            {
                if (place != value)
                    ChangePlace(value);

                place = value;
            }
        }

        public static LevelManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<LevelManager>();

                return instance;
            }
        }

        public event OnLoadingDelegate OnLoadingStart;
        public event OnLoadingDelegate OnLoadingFinish;

        public void InitSavedGame()
        {
            Init();

            SaveManager.LoadFloor();
            SaveManager.LoadPlayer(player);
            UIGame.Instance.RefreshPlayerInfo();
            SaveManager.LoadSpellsLauncher(player.GetComponent<SpellsLauncher>());
            SaveManager.LoadPlayerInventory(player.GetComponent<PlayerInventory>());
        }

        public void Init()
        {
            if (!playerGao)
            {
                playerGao = Instantiate(playerPrefab);
                playerGao.transform.SetParent(GameObject.FindGameObjectWithTag("DontDestroyGo").transform);
            }
            if (!guiMgr)
            {
                guiMgr = Instantiate(guiMgrPrefab);
                guiMgr.transform.SetParent(GameObject.FindGameObjectWithTag("DontDestroyGo").transform);
            }

            player = playerGao.GetComponent<Player>();

            uiGame = guiMgr.GetComponent<UIGame>();
            uiGame.Init(player);
            uiGame.transform.GetChild(1).GetComponent<Compass>().Init(player);
        }

        public void LoadMainMenu()
        {
            AsyncLoadLevel(0);
            Destroy(playerGao);
            playerGao = null;
            Destroy(guiMgr);
            guiMgr = null;
        }

        public void EnemyDetectPlayer(GameObject enemy)
        {
            uiGame.AddPointToCompass(enemy);
            player.BattleState = Character.Character.BATTLESTATES.InFight;
        }

        public void EnemyLostPlayer(GameObject enemy)
        {
            uiGame.RemovePointFromCompass(enemy);

            if (!uiGame.EnemyPointOnCompass())
                player.BattleState = Character.Character.BATTLESTATES.Neutral;
        }

        public void ChangePlace(PLACES place)
        {
            AsyncLoadLevel((int) place + 1);
        }

        public void LoadLevel(int sceneId)
        {
            mSceneId = sceneId;
            SceneManager.LoadScene(mSceneId);
        }

        public void LoadNextLevel()
        {
            if (mSceneId < SceneManager.sceneCountInBuildSettings - 1)
                mSceneId++;
            AsyncLoadLevel(mSceneId);
        }

        public void GameIsOver(string result)
        {
            uiGame.ShowResultScreen(result);
        }

        public void PlayerIsDied()
        {
            Destroy(GameObject.Find("EventSystem"));
            Destroy(GameObject.FindGameObjectWithTag("Player"));
            Destroy(GameObject.Find("UIGame(Clone)"));
            Cursor.lockState = CursorLockMode.None;
            LoadLevel(0);
            SaveManager.RemoveSaves();
        }

        public void AsyncLoadLevel(int sceneId)
        {
            if (uiGame)
            {
                player.BattleState = Character.Character.BATTLESTATES.Neutral;
                uiGame.RemovePointFromCompass(FindObjectOfType<Portal>().gameObject);
            }

            TimeManager.Instance.State = TimeManager.StateGame.Running;
            EnemyManager.Instance.DestroyAllEnemies();
            mSceneId = sceneId;
            StartCoroutine(AsyncLoadLevelCoroutine(mSceneId));
        }

        public void LoadNextFloor()
        {
            if (mSceneId != (int) PLACES.Dungeon + 1)
                place = PLACES.Dungeon;
            else
                FindObjectOfType<LevelGeneration>().NextFloor();
        }

        public void SetPlayerPos(Vector3 pos)
        {
            player.transform.position = pos;
        }

        private IEnumerator AsyncLoadLevelCoroutine(int sceneId)
        {
            if (OnLoadingStart != null)
                OnLoadingStart();
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneId);

            while (!async.isDone)
                yield return new WaitForSeconds(0.1f);

            if (OnLoadingFinish != null)
                OnLoadingFinish();
        }
    }
}