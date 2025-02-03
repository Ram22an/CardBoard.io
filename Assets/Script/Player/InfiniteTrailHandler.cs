using UnityEngine;
using System.Collections.Generic;

public class InfiniteTrailHandler : MonoBehaviour
{
    public GameObject sphereColliderPrefab;
    private float spawnInterval = 0.01f;
    private float colliderRadius = 0.5f;
    private float timeSinceLastSpawn;
    private List<Vector3> Positions = new List<Vector3>();
    public static InfiniteTrailHandler instance;
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }
    public void Update()
    {
        if (PlayerMovement.Instance.IsOutSide) {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnInterval)
            {
                SpawnCollider();
                timeSinceLastSpawn = 0f;
            }
        }
    }

    void SpawnCollider()
    {
        //Vector3 spawnPosition = transform.position;
        //GameObject newCollider = Instantiate(sphereColliderPrefab, spawnPosition, Quaternion.identity);
        //SphereCollider sphereCollider = newCollider.GetComponent<SphereCollider>();
        //sphereCollider.isTrigger = true;
        //sphereCollider.radius = colliderRadius;
        //Positions.Add(new Vector3(spawnPosition.x,0, spawnPosition.z));
        //newCollider.transform.parent = null;


        Vector3 spawnPosition = transform.position; // Get world position
        GameObject newCollider = Instantiate(sphereColliderPrefab, spawnPosition, Quaternion.identity);

        if (newCollider.TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
        {
            sphereCollider.isTrigger = true;
            sphereCollider.radius = colliderRadius;
        }
        else
        {
            Debug.LogWarning("SphereCollider component missing on prefab.");
        }

        Positions.Add(new Vector3(spawnPosition.x, 0, spawnPosition.z));

        newCollider.transform.parent = null;
    }
    public List<Vector2> GetPositions()
    {
        //return Positions;
        List<Vector2> vector2Positions = new List<Vector2>();
        foreach (var pos in Positions)
        {
            vector2Positions.Add(new Vector2(pos.x, pos.z));
        }
        return vector2Positions;
    }
}//class
