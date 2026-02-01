using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<int>    levelEnemiesCount = new List<int>() { 5 , 10 , 9999 };
    private int          currentLevelIndex;
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
        enemyToKillCountForLevel = levelEnemiesCount[levelIndex];
        switch (levelIndex)
        {
            case 0 :
                enemySpawners.StartSpawnFirstLevelEnemies(levelEnemiesCount[levelIndex]);
                break;
            case 1 :
                enemySpawners.StartSpawnAllEnemies(levelEnemiesCount[levelIndex]);
                break;
            case 2 :
                enemySpawners.StartSpawnForInfiniteLevels();
                break;
        }

    }

    private void EnemyDieEventHandler(object sender , EventArgs e)
    {
        enemyToKillCountForLevel -= 1;
        // clear current level
        if (enemyToKillCountForLevel == 0)
        {
            arena.OpenGate();
            currentLevelIndex++;
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

    public void GoNextLevel()
    {
        SpawnLevelEnemies(currentLevelIndex);
        // 刪除Masks
        foreach (var mask in FindObjectsByType<Mask>(FindObjectsInactive.Include , FindObjectsSortMode.InstanceID))
        {
            Destroy(mask.gameObject);
        }
    }
}