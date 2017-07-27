using System.Collections.Generic;
using Item;
using UI;
using UnityEngine;

namespace Managers
{
    public class EnemyManager : MonoBehaviour
    {
        private static EnemyManager instance;

        private readonly List<GameObject> enemies = new List<GameObject>();
        [SerializeField] private GameObject enemyPrefab;
        private GameObject enemySpawners;

        [SerializeField] private int stepConstitution = 2;

        [SerializeField] private int stepDexterity = 2;

        [SerializeField] private int stepIntelligence = 2;

        [SerializeField] private int stepStrength = 2;

        public static EnemyManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<EnemyManager>();

                return instance;
            }
        }

        public GameObject EnemySpawners
        {
            set { enemySpawners = value; }
        }

        public void CreateAllEnemies(int nbEnemies)
        {
            DestroyAllEnemies();

            int playerLevel = GameManager.Instance.PlayerInstance.GetComponent<Character.Character>().Level;

            if (enemySpawners && enemySpawners.transform.childCount > 0)
            {
                if (nbEnemies > enemySpawners.transform.childCount)
                    nbEnemies = enemySpawners.transform.childCount;

                int nbSpawners = enemySpawners.transform.childCount;

                for (int count = 0; count < nbEnemies; count++)
                {
                    int rand = Random.Range(1, nbSpawners);

                    GameObject enemy = Instantiate(enemyPrefab);
                    GameObject spawner = enemySpawners.transform.GetChild(rand - 1).gameObject;

                    nbSpawners -= 1;
                    spawner.transform.parent = null;
                    spawner.transform.parent = enemySpawners.transform;
                    enemy.transform.SetParent(transform);
                    enemy.transform.position = spawner.transform.position;
                    enemy.name = enemyPrefab.name;

                    FillEnemyLoot(enemy, 3);

                    IncreaseStatsBasedOnLevel(enemy, playerLevel);

                    enemies.Add(enemy);
                }
            }
        }

        public void DestroyAllEnemies()
        {
            for (int count = enemies.Count; count > 0; count--)
                DestroyEnemy(enemies[count - 1]);
        }

        public void DestroyEnemy(GameObject enemy)
        {
            if (enemy)
                UIGame.Instance.RemovePointFromCompass(enemy);

            enemies.Remove(enemy);
            Destroy(enemy);
        }

        public GameObject CreateBoss(GameObject bossPrefab)
        {
            GameObject boss = Instantiate(bossPrefab);

            int playerLevel = GameManager.Instance.PlayerInstance.GetComponent<Character.Character>().Level;

            IncreaseStatsBasedOnLevel(boss, playerLevel);

            Character.Character bossStats = boss.GetComponent<Character.Character>();
            bossStats.Strength *= 2;
            bossStats.Constitution *= 2;
            bossStats.Dexterity *= 2;
            bossStats.Intelligence *= 2;

            ItemData skull = Resources.Load<ItemData>("Items/Skull");
            LootInventory loot = boss.GetComponent<LootInventory>();
            loot.Items.Add(skull);


            enemies.Add(boss);

            return boss;
        }

        private void FillEnemyLoot(GameObject enemy, int maxCount)
        {
            LootInventory loot = enemy.GetComponent<LootInventory>();

            ItemData[] items = Resources.LoadAll<ItemData>("Items/");

            ItemData item = items[Random.Range(0, items.Length)];
            if (!item.isQuestItem)
                loot.Items.Add(item);

            for (int i = 1; i < maxCount; i++)
            {
                if (Random.Range(0, 2) == 0)
                    return;
                item = items[Random.Range(0, items.Length)];
                if (!item.isQuestItem)
                    loot.Items.Add(item);
            }
        }

        public void IncreaseStatsBasedOnLevel(GameObject enemy, int level)
        {
            Character.Character c = enemy.GetComponent<Character.Character>();
            c.Strength += stepStrength * level;
            c.Dexterity += stepDexterity * level;
            c.Constitution += stepConstitution * level;
            c.Intelligence += stepIntelligence * level;
            c.RecalculateCharacterStats();
        }
    }
}