#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

/// <summary>
///     關卡管理
/// </summary>
public class LevelManager : MonoBehaviour
{
#region Private Variables

    private List<int>    levelEnemiesCount = new List<int>() { 5 , 10 , 9999 };
    private int          currentLevelIndex;
    private int          enemyToKillCountForLevel;
    private Arena        arena;
    private EnemySpawner enemySpawners;

    [SerializeField]
    private int initLevel = 1;

#endregion

#region Unity events

    void Start()
    {
        currentLevelIndex = Mathf.Min(levelEnemiesCount.Count - 1 , initLevel);
        arena             = FindFirstObjectByType<Arena>();
        enemySpawners     = FindFirstObjectByType<EnemySpawner>();
        SpawnLevelEnemies(currentLevelIndex);
        Enemy.DieEventHandler += EnemyDieEventHandler;
    }

#endregion

#region Public Methods

    public void GoNextLevel()
    {
        SpawnLevelEnemies(currentLevelIndex);
        // 刪除Masks
        foreach (var mask in FindObjectsByType<Mask>(FindObjectsInactive.Include , FindObjectsSortMode.InstanceID))
            Destroy(mask.gameObject);
    }

    public bool IsLastLevel()
    {
        return currentLevelIndex >= levelEnemiesCount.Count;
    }

#endregion

#region Private Methods

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

#endregion
}