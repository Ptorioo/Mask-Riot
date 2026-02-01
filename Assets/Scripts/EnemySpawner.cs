#region

using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class EnemySpawner : MonoBehaviour
{
#region Private Variables

    [SerializeField]
    private GameObject[] enemyPrefab;

    [SerializeField]
    private float minX;

    [SerializeField]
    private float maX;

    private int firstEnemySpawnTime        = 6;
    private int secondOrNextEnemySpawnRate = 3;

#endregion

#region Unity events

#endregion

#region Private Methods

    private void SpawnEnemyFor1Type()
    {
        var prefab = enemyPrefab[0];
        SpawnEnemy(prefab);
    }

    private void SpawnEnemyForAll()
    {
        var prefab = enemyPrefab[Random.Range(0 , enemyPrefab.Length)];
        SpawnEnemy(prefab);
    }

    private void SpawnEnemy(GameObject prefab)
    {
        var x   = Random.Range(minX , maX);
        var pos = new Vector3(x , -3.6f , 0);
        Instantiate(prefab , pos , quaternion.identity);
    }

#endregion

    [ContextMenu("測試產生10隻，第一關怪物")]
    private void TestStartSpawnFirstLevelEnemies()
    {
        StartSpawnFirstLevelEnemies(10);
    }

    [ContextMenu("測試產生20隻，第二關怪物")]
    private void TestStartSpawnLevel2Enemies()
    {
        StartSpawnAllEnemies(20);
    }

    public void StartSpawnFirstLevelEnemies(int enemyCount)
    {
        InvokeRepeating(nameof(SpawnEnemyFor1Type) , firstEnemySpawnTime , secondOrNextEnemySpawnRate);

        // 全部產生完畢，取消怪物產生
        Invoke(nameof(CancelSpawnFor1Type) , 6 + enemyCount * secondOrNextEnemySpawnRate);
    }

    public void StartSpawnAllEnemies(int enemyCount)
    {
        InvokeRepeating(nameof(SpawnEnemyForAll) , firstEnemySpawnTime , secondOrNextEnemySpawnRate);

        // 全部產生完畢，取消怪物產生
        Invoke(nameof(CancelSpawnAll) , 6 + enemyCount * secondOrNextEnemySpawnRate);
    }

    private float infiniteLevelSpawnTime = 6f;

    public void StartSpawnForInfiniteLevels()
    {
        SpawnEnemyForAll();
        Invoke(nameof(StartSpawnForInfiniteLevels) , infiniteLevelSpawnTime);
        infiniteLevelSpawnTime *= 0.95f;
        infiniteLevelSpawnTime =  Mathf.Max(0.1f , infiniteLevelSpawnTime);
    }

    private void CancelSpawnFor1Type()
    {
        CancelInvoke(nameof(SpawnEnemyFor1Type));
    }

    private void CancelSpawnAll()
    {
        CancelInvoke(nameof(SpawnEnemyForAll));
    }
}