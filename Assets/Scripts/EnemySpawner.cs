using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> Enemies;
    [SerializeField] private Vector3 SpawnRange;
    [SerializeField] private float SpawnRate;
    [SerializeField] private bool Spawn = true;
    

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (Spawn)
        {
            var position = new Vector3(Random.Range(-SpawnRange.x, SpawnRange.x),
                Random.Range(-SpawnRange.y, SpawnRange.y),
                Random.Range(-SpawnRange.z, SpawnRange.z));
            var enemyNumber = Random.Range(0, Enemies.Count);
            Instantiate(Enemies[enemyNumber],position,Quaternion.identity);
            yield return new WaitForSeconds(SpawnRate);
        }
    }
}
