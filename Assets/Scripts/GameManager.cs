using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private List<int>    levelEnemiesCount = new List<int>() { 10 , 20 };
    private int          currentLevelIndex = 0;
    private int          enemyToKillCountForLevel;
    private Arena        arena;
    private EnemySpawner enemySpawners;

    void Start()
    {
        arena         = FindFirstObjectByType<Arena>();
        enemySpawners = FindFirstObjectByType<EnemySpawner>();
        SpawnLevelEnemies(currentLevelIndex);
        Enemy.DieEventHandler += EnemyDieEventHandler;
    }

    private void SpawnLevelEnemies(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0 :
                enemySpawners.StartSpawnFirstLevelEnemies(levelEnemiesCount[levelIndex]);
                break;
            case 1 :
                enemySpawners.StartSpawnAllEnemies(levelEnemiesCount[levelIndex]);
                break;
        }

        enemyToKillCountForLevel = levelEnemiesCount[levelIndex];
    }

    private void EnemyDieEventHandler(object sender , EventArgs e)
    {
        enemyToKillCountForLevel -= 1;
        // clear current level
        if (enemyToKillCountForLevel == 0)
        {
            arena.OpenGate();
            currentLevelIndex++;
            // game clear
            if (currentLevelIndex >= levelEnemiesCount.Count)
            {
                return;
            }

            SpawnLevelEnemies(currentLevelIndex);
        }
    }

    private void OnDestroy()
    {
        Enemy.DieEventHandler -= EnemyDieEventHandler;
    }

    public bool IsLastLevel()
    {
        return currentLevelIndex >= levelEnemiesCount.Count;
    }
}