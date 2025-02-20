using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class ResourceGenerator : MonoBehaviour
{
    [Header("Spawn settings")]
    public List<GameObject> resourcePrefabs; // Список префабов ресурсов
    public float spawnChance;
 
    [Header("Raycast setup")]
    public float distanceBetweenCheck;
    public float heightOfCheck = 10f, rangeOfCheck = 30f;
    public LayerMask layerMask;
    public Vector2 positivePosition, negativePosition;
    
    private void Start()
    {
        SpawnResources();
    }
 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnResources();
        }
    }
 
    void SpawnResources()
    {
        if (resourcePrefabs.Count == 0)
        {
            Debug.LogWarning("Нет ресурсов для спавна!");
            return;
        }

        for (float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
        {
            for (float z = negativePosition.y; z < positivePosition.y; z += distanceBetweenCheck)
            {
                RaycastHit hit;
                if (Physics.Raycast(new Vector3(x, heightOfCheck, z), Vector3.down, out hit, rangeOfCheck, layerMask))
                {
                    if (spawnChance > Random.Range(0f, 101f))
                    {
                        // Выбираем случайный префаб из списка
                        GameObject randomPrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Count)];
                        Instantiate(randomPrefab, hit.point, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), transform);
                    }
                }
            }
        }
    }
}