using System;
using System.CodeDom.Compiler;
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

   /* private void Awake()
    {
        Instance = this;
    }*/

   private void Start()
   {
       generatedRooms = new List<GenLevelPart>();
       StartGeneration();
   }

   public void StartGeneration()
   {
       Generate();
       GenerateAlternateEntrances();
       isGenerated = true;
   }

   private void Generate()
   {
       for (int i = 0; i < maxCountRooms - alternateEntrances.Count; i++)
       {
           if (generatedRooms.Count < 1)
           {
               GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation);
               
               generatedRoom.transform.SetParent(null);
               if (generatedRoom.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
               {
                  // genLevelPart.GetNetworkObject().Spawn(true);
                   generatedRooms.Add(genLevelPart);
               }
           }
           else
           {
               bool isPlaceHallway = Random.Range(0f, 1f) > 0.5f;
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
                       break;
                   }

                   retryIndex++;
               }

               GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);
               //doorToAlign.GetComponent<NetworkObject>().Spawn(true);

               if (isPlaceHallway)
               {
                   int randomIndex = Random.Range(0, hallways.Count);
                   GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);
                   generatedHallway.transform.SetParent(null);
                   if (generatedHallway.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
                   {
                       //genLevelPart.GetNetworkObject().Spawn(true);
                       if (genLevelPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
                       {
                           generatedRooms.Add(genLevelPart);
                           doorToAlign.transform.position = roomEntrypoint.transform.position;
                           doorToAlign.transform.rotation = roomEntrypoint.transform.rotation;
                           AlignRooms(randomRoom.transform, generatedHallway.transform, roomEntrypoint,
                               room2Entrypoint);
                           if (HandleIntersection(genLevelPart))
                           {
                               genLevelPart.UnuseEntrypoint(roomEntrypoint);
                               randomRoom.UnuseEntrypoint(roomEntrypoint);
                               RetryPlacement(generatedHallway, doorToAlign);
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
                       if (isPlaceSpecialRoom)
                       {
                           int randomIndex = Random.Range(0, specialRooms.Count);
                           generatedRoom = Instantiate(specialRooms[randomIndex], transform.position,
                               transform.rotation);

                       }
                       else
                       {
                           int randomIndex = Random.Range(0, rooms.Count);
                           generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                       }
                   }
                   else
                   {
                       int randomIndex = Random.Range(0, rooms.Count);
                       generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                   }
                   generatedRoom.transform.SetParent(null);

                   if (generatedRoom.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
                   {
                       //genLevelPart.GetNetworkObject().Spawn(true);
                       if (genLevelPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
                       {
                           generatedRooms.Add(genLevelPart);
                           doorToAlign.transform.position = roomEntrypoint.transform.position;
                           doorToAlign.transform.rotation = roomEntrypoint.transform.rotation;
                           AlignRooms(randomRoom.transform, generatedRoom.transform, roomEntrypoint, room2Entrypoint);

                           if (HandleIntersection(genLevelPart))
                           {
                               genLevelPart.UnuseEntrypoint(room2Entrypoint);
                               randomRoom.UnuseEntrypoint(roomEntrypoint);
                               RetryPlacement(generatedRoom, doorToAlign);
                               continue;
                           }
                       }
                   }
               }
           }
       }
   }

   private void GenerateAlternateEntrances()
   {
       
   }

   private void FillEmptyEntrances()
   {
       generatedRooms.ForEach(room => room.FillEmptyDoors());
   }

   private void AlignRooms(Transform room1, Transform room2, Transform room1Entry, Transform room2Entry)
   {
       float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);
       room2.TransformPoint(room2Entry.position);
       room2.eulerAngles = new Vector3(room2.eulerAngles.x, room2.eulerAngles.y, room2.eulerAngles.z);

       Vector3 offset = room1Entry.position - room2Entry.position;

       room2.position += offset;
       Physics.SyncTransforms();
   }

   private bool HandleIntersection(GenLevelPart genLevelPart)
   {
       bool didIntersect = false;
       Collider[] hits = Physics.OverlapBox(genLevelPart.collider.bounds.center, genLevelPart.collider.bounds.size / 2,
           Quaternion.identity, roomsLayerMask);
       foreach (Collider hit in hits)
       {
           if (hit == genLevelPart.collider) continue;
           if (hit != genLevelPart.collider)
           {
               didIntersect = true;
               break;
           }
       }

       return didIntersect;
   }

   private void RetryPlacement(GameObject itemPlace, GameObject doorToPlace)
   {
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
               break;
           }

           retryIndex++;
       }

       if (itemPlace.TryGetComponent<GenLevelPart>(out GenLevelPart genLevelPart))
       {
           if (genLevelPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
           {
               doorToPlace.transform.position = room1Entrypoint.transform.position;
               doorToPlace.transform.rotation = room1Entrypoint.transform.rotation;
                AlignRooms(randomGeneratedRoom.transform, itemPlace.transform, room1Entrypoint, room2Entrypoint);

                if (HandleIntersection(genLevelPart))
                {
                    genLevelPart.UnuseEntrypoint(room2Entrypoint);
                    randomGeneratedRoom.UnuseEntrypoint(room1Entrypoint);
                    RetryPlacement(itemPlace, doorToPlace);
                }
           }
       }
   }
}
