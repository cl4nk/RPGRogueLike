using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int enemyNumber;

    [SerializeField] private GameObject enemySpawners;

    [SerializeField] private Transform posPortal;

    private void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        player.position = posPortal.position;
        player.forward = posPortal.right;

        UIGame.Instance.AddPointToCompass(posPortal.parent.gameObject);
        if (enemySpawners == null)
            return;
        EnemyManager.Instance.EnemySpawners = enemySpawners;
        EnemyManager.Instance.CreateAllEnemies(enemyNumber);
    }
}