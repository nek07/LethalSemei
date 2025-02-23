using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenLevelPart : MonoBehaviour
{
    public enum PartType
    {
        Room,
        Hallway
    }

    [SerializeField] private LayerMask roomsLayerMask;
    [SerializeField] private PartType partType;
    [SerializeField] private GameObject fillerWall;
    public List<Transform> entrypoints;
    public List<Collider> colliders = new List<Collider>(); // Теперь список коллайдеров

    public bool HasAvailableEntrypoint(out Transform entrypoint)
    {
        Transform resultingEntry = null;
        bool result = false;

        int totalRetries = 100;
        int retryIndex = 0;

        while (resultingEntry == null && retryIndex < totalRetries)
        {
            int randomEntryIndex = Random.Range(0, entrypoints.Count);
            Transform entry = entrypoints[randomEntryIndex];

            if (entry.TryGetComponent<EntryPoint>(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    resultingEntry = entry;
                    result = true;
                    entryPoint.SetOccupied();
                    break;
                }
            }

            retryIndex++;
        }

        entrypoint = resultingEntry;
        return result;
    }

    public void UnuseEntrypoint(Transform entrypoint)
    {
        if (entrypoint.TryGetComponent(out EntryPoint entry))
        {
            entry.SetOccupied(false);
        }
    }

    public void FillEmptyDoors()
    {
        foreach (var entry in entrypoints)
        {
            if (entry.TryGetComponent(out EntryPoint entryPoint) && !entryPoint.IsOccupied())
            {
                GameObject wall = Instantiate(fillerWall);
                wall.transform.position = entry.transform.position;
                wall.transform.rotation = entry.transform.rotation;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = partType == PartType.Room ? Color.magenta : Color.yellow;

        foreach (var col in colliders)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
