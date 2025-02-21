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
    public Vector3 positivePosition, negativePosition; // Изменено с Vector2 на Vector3

    private void Start()
    {
        StartCoroutine(SpawnResources());
    }

    IEnumerator SpawnResources()
    {
        yield return new WaitForSeconds(5f); // Ждём перед спавном
        Debug.Log("IMHERE");

        if (resourcePrefabs.Count == 0)
        {
            Debug.LogWarning("Нет ресурсов для спавна!");
            yield break; // Останавливаем корутину, если нечего спавнить
        }

        // Проход по X, Y, Z для трехмерного распределения
        for (float x = negativePosition.x; x < positivePosition.x; x += distanceBetweenCheck)
        {
            for (float y = negativePosition.y; y < positivePosition.y; y += distanceBetweenCheck) // Учитываем Y
            {
                for (float z = negativePosition.z; z < positivePosition.z; z += distanceBetweenCheck)
                {
                    RaycastHit hit;
                    Vector3 checkPosition = new Vector3(x, y, z);

                    if (Physics.Raycast(checkPosition, Vector3.down, out hit, rangeOfCheck, layerMask))
                    {
                        if (spawnChance > Random.Range(0f, 101f))
                        {
                            GameObject randomPrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Count)];
                            Instantiate(randomPrefab, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                        }
                    }
                }
            }
        }

        Debug.Log("DONEEEEEEEEEEE");
    }
}
