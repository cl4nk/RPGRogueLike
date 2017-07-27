using UnityEngine;

public class BootManager : MonoBehaviour
{
    private static bool isLoaded;
    [SerializeField] private GameObject alteractionMgrPrefab;
    [SerializeField] private GameObject audioMgrPrefab;
    [SerializeField] private GameObject dontDestroyPrefab;
    [SerializeField] private GameObject enemyMgrPrefab;
    [SerializeField] private GameObject gameMgrPrefab;
    [SerializeField] private GameObject goalMgrPrefab;
    [SerializeField] private GameObject guiMgrPrefab;
    [SerializeField] private GameObject levelMgrPrefab;
    [SerializeField] private GameObject previewRoomPrefab;
    [SerializeField] private GameObject timeMgrPrefab;

    private void Start()
    {
        if (!isLoaded)
        {
            GameObject dontDestroyGo = Instantiate(dontDestroyPrefab);
            GameObject gameMgr = Instantiate(gameMgrPrefab);
            GameObject audioMgr = Instantiate(audioMgrPrefab);
            GameObject levelMgr = Instantiate(levelMgrPrefab);
            GameObject enemyMgr = Instantiate(enemyMgrPrefab);
            GameObject timeMgr = Instantiate(timeMgrPrefab);
            GameObject alteractionMgr = Instantiate(alteractionMgrPrefab);
            GameObject goalMgr = Instantiate(goalMgrPrefab);
            GameObject previewRoom = Instantiate(previewRoomPrefab);

            DontDestroyOnLoad(dontDestroyGo);

            isLoaded = true;

            gameMgr.transform.parent = dontDestroyGo.transform;
            audioMgr.transform.parent = dontDestroyGo.transform;
            levelMgr.transform.parent = dontDestroyGo.transform;
            enemyMgr.transform.parent = dontDestroyGo.transform;
            timeMgr.transform.parent = dontDestroyGo.transform;
            alteractionMgr.transform.parent = dontDestroyGo.transform;
            goalMgr.transform.parent = dontDestroyGo.transform;
            previewRoom.transform.parent = dontDestroyGo.transform;
        }
    }
}