using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[RequireComponent(typeof(TimeAgent))]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] Item ItemToSpawn;
    [SerializeField] int Count;
    [SerializeField] float Spread;
    [SerializeField] float Probability = 0.5f;

    private void Start()
    {
        TimeAgent timeAgent = GetComponent<TimeAgent>();
        timeAgent.OnTimeTick += Spawn;
    }
    void Spawn(DayTimeController dayTimeController)
    {
        if(Random.value < Probability)
        {
            Vector3 position = transform.position;
            position.x += Spread * Random.value - Spread / 2;
            position.y += Spread * Random.value - Spread / 2;
            ItemSpawManager.Instance.SpawnItem(this.transform, position, ItemToSpawn, Count);
        }
    }
}
