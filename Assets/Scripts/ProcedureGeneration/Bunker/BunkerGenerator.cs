using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BunkerGenerator : MonoBehaviour
{
    [SerializeField] private GameObject entrance;
    [SerializeField] private List<GameObject> rooms;
    [SerializeField] private List<GameObject> specialRooms;
    [SerializeField] private List<GameObject> alternateEntrances;
    [SerializeField] private List<GameObject> hallways;
    [SerializeField] private GameObject door;
    [SerializeField] private int maxCountRooms;
    [SerializeField] private LayerMask roomsLayerMask;

    private List<GenLevelPart> generatedRooms;
    private bool isGenerated = false;

    private void Start()
    {
        generatedRooms = new List<GenLevelPart>();
        StartGeneration();
    }

    public void StartGeneration()
    {
        Debug.Log("Starting generation...");
        Generate();
        //GenerateAlternateEntrances();
        isGenerated = true;
        Debug.Log("Generation finished.");
        gameObject.SetActive(false);
        
    }

    private void Generate()
    {
        for (int i = 0; i < maxCountRooms - alternateEntrances.Count; i++)
        {
            Debug.Log($"Generating room {i + 1}/{maxCountRooms}...");
            if (generatedRooms.Count < 1)
            {
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation);
                Debug.Log("Entrance room generated.");

                generatedRoom.transform.SetParent(null);
                if (generatedRoom.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
                {
                    generatedRooms.Add(genLevelPart);
                    Debug.Log("Entrance room added to the generated rooms list.");
                }
            }
            else
            {
                bool isPlaceHallway = Random.Range(0f, 1f) > 0.5f;
                Debug.Log($"Placing {(isPlaceHallway ? "hallway" : "room")}...");

                GenLevelPart randomRoom = null;
                Transform roomEntrypoint = null;
                int totalRetries = 100;
                int retryIndex = 0;

                while (randomRoom == null && retryIndex < totalRetries)
                {
                    int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count);
                    GenLevelPart testRoom = generatedRooms[randomLinkRoomIndex];
                    if (testRoom.HasAvailableEntrypoint(out roomEntrypoint))
                    {
                        randomRoom = testRoom;
                        Debug.Log("Found available entry point in a generated room.");
                        break;
                    }

                    retryIndex++;
                    if (retryIndex == totalRetries)
                    {
                        Debug.LogWarning("Failed to find available entry point after maximum retries.");
                    }
                }

                // doorToAlign = Instantiate(door, transform.position, transform.rotation);
                //Debug.Log("Door instantiated.");

                if (isPlaceHallway)
                {
                    int randomIndex = Random.Range(0, hallways.Count);
                    GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);
                    Debug.Log($"Hallway {randomIndex} generated.");

                    generatedHallway.transform.SetParent(null);
                    if (generatedHallway.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
                    {
                        if (genLevelPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
                        {
                            generatedRooms.Add(genLevelPart);
                            AlignRooms(randomRoom.transform, generatedHallway.transform, roomEntrypoint, room2Entrypoint);

                            if (HandleIntersection(genLevelPart))
                            {
                                Debug.LogWarning("Intersection detected, retrying hallway placement...");
                                genLevelPart.UnuseEntrypoint(room2Entrypoint);
                                randomRoom.UnuseEntrypoint(roomEntrypoint);
                                RetryPlacement(generatedHallway);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    GameObject generatedRoom;

                    if (specialRooms.Count > 0)
                    {
                        bool isPlaceSpecialRoom = Random.Range(0f, 1f) > 0.9f;
                        int randomIndex = isPlaceSpecialRoom
                            ? Random.Range(0, specialRooms.Count)
                            : Random.Range(0, rooms.Count);

                        generatedRoom = Instantiate(isPlaceSpecialRoom ? specialRooms[randomIndex] : rooms[randomIndex],
                            transform.position, transform.rotation);

                        Debug.Log($"{(isPlaceSpecialRoom ? "Special room" : "Regular room")} generated.");
                    }
                    else
                    {
                        int randomIndex = Random.Range(0, rooms.Count);
                        generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        Debug.Log($"Room {randomIndex} generated.");
                    }

                    generatedRoom.transform.SetParent(null);

                    if (generatedRoom.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
                    {
                        if (genLevelPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
                        {
                            generatedRooms.Add(genLevelPart);
                            AlignRooms(randomRoom.transform, generatedRoom.transform, roomEntrypoint, room2Entrypoint);

                            if (HandleIntersection(genLevelPart))
                            {
                                Debug.LogWarning("Intersection detected, retrying room placement...");
                                genLevelPart.UnuseEntrypoint(room2Entrypoint);
                                randomRoom.UnuseEntrypoint(roomEntrypoint);
                                RetryPlacement(generatedRoom);
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    private void AlignRooms(Transform room1, Transform room2, Transform room1Entry, Transform room2Entry)
    {
        Debug.Log("Aligning rooms...");
        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);
        room2.TransformPoint(room2Entry.position);
        room2.eulerAngles = new Vector3(room2.eulerAngles.x, room2.eulerAngles.y + angle, room2.eulerAngles.z);

        Vector3 offset = room1Entry.position - room2Entry.position;
        room2.position += offset;
        Physics.SyncTransforms();
        Debug.Log("Rooms aligned.");
    }

    private bool HandleIntersection(GenLevelPart genLevelPart)
    {
        Collider[] hits = Physics.OverlapBox(genLevelPart.collider.bounds.center, genLevelPart.collider.bounds.size / 2,
            Quaternion.identity, roomsLayerMask);
        foreach (Collider hit in hits)
        {
            if (hit != genLevelPart.collider)
            {
                Debug.LogWarning($"Intersection detected with {hit.gameObject.name}.");
                return true;
            }
        }

        return false;
    }

    private int retryAttempts = 0;
    private const int maxRetryAttempts = 10;

    private void RetryPlacement(GameObject itemPlace)
    {
        if (retryAttempts >= maxRetryAttempts)
        {
            Debug.LogWarning("Maximum retry attempts reached. Stopping further retries to prevent infinite loop.");
            return;
        }

        retryAttempts++;
        Debug.Log($"Retrying placement... Attempt {retryAttempts}/{maxRetryAttempts}");

        GenLevelPart randomGeneratedRoom = null;
        Transform room1Entrypoint = null;
        int totalRetries = 100;
        int retryIndex = 0;

        while (randomGeneratedRoom == null && retryIndex < totalRetries)
        {
            int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count - 1);
            GenLevelPart testRoom = generatedRooms[randomLinkRoomIndex];
            if (testRoom.HasAvailableEntrypoint(out room1Entrypoint))
            {
                randomGeneratedRoom = testRoom;
                Debug.Log("Found available entry point in a generated room for retry.");
                break;
            }

            retryIndex++;
        }

        if (itemPlace.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
        {
            if (genLevelPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
            {
                AlignRooms(randomGeneratedRoom.transform, itemPlace.transform, room1Entrypoint, room2Entrypoint);

                if (HandleIntersection(genLevelPart))
                {
                    Debug.LogWarning("Intersection detected during retry, retrying again...");
                    genLevelPart.UnuseEntrypoint(room2Entrypoint);
                    randomGeneratedRoom.UnuseEntrypoint(room1Entrypoint);
                    RetryPlacement(itemPlace);
                }
            }
        }

        retryAttempts = 0; // Сбросить счетчик после завершения
    }
}
