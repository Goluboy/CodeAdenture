using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Spawnable objectToSpawn;

    private void Awake()
    {
        SpawnNew();
    }

    public void SpawnNew()
    {
        Instantiate(objectToSpawn, transform.position, new Quaternion()).gameObject.AddComponent<BoxCollider2D>().transform.parent = transform;
    }
}
