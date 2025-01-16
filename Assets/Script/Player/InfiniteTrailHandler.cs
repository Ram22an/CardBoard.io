using UnityEngine;
using System.Collections.Generic;

public class InfiniteTrailHandler : MonoBehaviour
{
    public GameObject sphereColliderPrefab;
    private float spawnInterval = 0.001f;
    private float colliderRadius = 0.5f;

    private float timeSinceLastSpawn;

    public void Update()
    { 
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnCollider();
            timeSinceLastSpawn = 0f;
        }
    }

    void SpawnCollider()
    {
        Vector3 spawnPosition = transform.position;
        GameObject newCollider = Instantiate(sphereColliderPrefab, spawnPosition, Quaternion.identity);
        SphereCollider sphereCollider = newCollider.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = colliderRadius;
        newCollider.transform.parent = null;
    }
}
