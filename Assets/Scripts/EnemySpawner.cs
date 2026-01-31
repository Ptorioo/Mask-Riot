#region

using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class EnemySpawner : MonoBehaviour
{
#region Private Variables

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private float minX;

    [SerializeField]
    private float maX;

#endregion

#region Unity events

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy) , 3 , 3);
    }

#endregion

#region Private Methods

    private void SpawnEnemy()
    {
        var x   = Random.Range(minX , maX);
        var pos = new Vector3(x , -3.6f , 0);
        Instantiate(enemyPrefab , pos , quaternion.identity);
    }

#endregion
}