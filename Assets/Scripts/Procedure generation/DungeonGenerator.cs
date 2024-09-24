using System;
using System.Collections;
using System.Collections.Generic;
using Graphs;
using UnityEngine;
using Random = System.Random;
using UnityEngine.SceneManagement;
using Vertex = Graphs.Vertex;

public class DungeonGenerator : MonoBehaviour
{
    enum CellType
    {
        None,
        Room,
        Hallway,
        Stairs
    }

    class Room
    {
        public Vector3 location;
        public GameObject prefab;
        public Bounds bounds;

        public Room(Vector3 location, GameObject gameObject)
        {
            this.location = location;
            this.prefab = gameObject;
            this.bounds = new Bounds(location, GetPrefabSize(gameObject));
        }  
        public Room(Vector3 location, Vector3 size)
        {
            this.location = location;
            this.bounds = new Bounds(location, size);
        }
        
        public static bool Intersect(Room a, Room b)
        {
            return
                a.bounds.Intersects(b.bounds);
        }
    }
    
    [SerializeField] private Vector3 size;
    [SerializeField] private int maxRoomCount;
    [SerializeField] private Transform[] fireExitsPositions;
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private int[] roomPrefabMaxCount;
    [SerializeField] private GameObject[] corridorPrefabs;
    [SerializeField] private GameObject[] stairsPrefabs;

    private Random random;
    private Grid3D<CellType> grid;
    private List<Room> rooms;
    Delaunay delaunay;
    private HashSet<Prim.Edge> selectedEdges;


    private void Start()
    {
        random = new Random();
        grid = new Grid3D<CellType>(size, Vector3.zero);
        rooms = new List<Room>();
        
        GenerateDungeon();
        
    }

    private void GenerateDungeon()
    {
        PlaceRooms();
        Triangulate();
        CreateHallways();
        PathfindHallways();
    }

    private void PlaceRooms()
    {
        int count = 0;
        for (int i = 0; i < maxRoomCount; i++)
        {
            Vector3 location = new Vector3(
                (float)(random.NextDouble() * size.x),
                (float)(random.NextDouble() * size.y),
                (float)(random.NextDouble() * size.z));

            GameObject roomPref = roomPrefabs[random.Next(0, roomPrefabs.Length)];
            bool add = true;
            Room newRoom = new Room(location, roomPref);
            Room buffer = new Room(location, GetPrefabSize(roomPref) + new Vector3(0.4f, 0, 0.4f));

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            /*if (newRoom.bounds.min.x < 0 || newRoom.bounds.max.x >= size.x
                                         || newRoom.bounds.min.y < 0 || newRoom.bounds.max.y >= size.y
                                         || newRoom.bounds.min.z < 0 || newRoom.bounds.max.z >= size.z)
            {
                add = false;
            }*/

            if (add)
            {
                Debug.Log(++count);
                rooms.Add(newRoom);
                PlaceRoom(newRoom.location);
                
                
                


            }
            
        }
    }
    void Triangulate() {
        Debug.Log(rooms.Count + " - rooms");
        List<Vertex> vertices = new List<Vertex>();
        
        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector3)room.location + ((Vector3)room.bounds.size) / 2, room));
        }
 
        delaunay = Delaunay.Triangulate(vertices);
    }
    
    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();
 
        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }
 
        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);
 
        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);
 
        foreach (var edge in remainingEdges) {
            if (random.NextDouble() < 0.3) {
                selectedEdges.Add(edge);
            }
        }
    }
    
    
     void PathfindHallways() {
         PathFinder aStar = new PathFinder(size);
 
         foreach (var edge in selectedEdges) {
             var startRoom = (edge.U as Vertex<Room>).Item;
             var endRoom = (edge.V as Vertex<Room>).Item;
 
             var startPosf = startRoom.bounds.center;
             var endPosf = endRoom.bounds.center;
             var startPos = new Vector3((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
             var endPos = new Vector3((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);
 
             var path = aStar.FindPath(startPos, endPos, (PathFinder.Node a, PathFinder.Node b) => {
                 var pathCost = new PathFinder.PathCost();
 
                 var delta = b.Position - a.Position;
 
                 if (delta.y == 0) {
                     //flat hallway
                     pathCost.cost = Vector3.Distance(b.Position, endPos);    //heuristic
 
                     if (grid[b.Position] == CellType.Stairs) {
                         return pathCost;
                     } else if (grid[b.Position] == CellType.Room) {
                         pathCost.cost += 5;
                     } else if (grid[b.Position] == CellType.None) {
                         pathCost.cost += 1;
                     }
 
                     pathCost.traversable = true;
                 } else {
                     //staircase
                     if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                         || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;
 
                     pathCost.cost = 100 + Vector3.Distance(b.Position, endPos);    //base cost + heuristic
 
                     float xDir = Mathf.Clamp(delta.x, -1, 1);
                     float zDir = Mathf.Clamp(delta.z, -1, 1);
                     Vector3 verticalOffset = new Vector3(0, delta.y, 0);
                     Vector3 horizontalOffset = new Vector3(xDir, 0, zDir);
 
                     if (!grid.InBounds(a.Position + verticalOffset)
                         || !grid.InBounds(a.Position + horizontalOffset)
                         || !grid.InBounds(a.Position + verticalOffset + horizontalOffset)) {
                         return pathCost;
                     }
 
                     if (grid[a.Position + horizontalOffset] != CellType.None
                         || grid[a.Position + horizontalOffset * 2] != CellType.None
                         || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                         || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None) {
                         return pathCost;
                     }
 
                     pathCost.traversable = true;
                     pathCost.isStairs = true;
                 }
 
                 return pathCost;
             });
 
             if (path != null) {
                 for (int i = 0; i < path.Count; i++) {
                     var current = path[i];
 
                     if (grid[current] == CellType.None) {
                         grid[current] = CellType.Hallway;
                     }
 
                     if (i > 0) {
                         var prev = path[i - 1];
 
                         var delta = current - prev;
 
                         if (delta.y != 0) {
                             float xDir = Mathf.Clamp(delta.x, -1, 1);
                             float zDir = Mathf.Clamp(delta.z, -1, 1);
                             Vector3 verticalOffset = new Vector3(0, delta.y, 0);
                             Vector3 horizontalOffset = new Vector3(xDir, 0, zDir);
                             
                             grid[prev + horizontalOffset] = CellType.Stairs;
                             grid[prev + horizontalOffset * 2] = CellType.Stairs;
                             grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                             grid[prev + verticalOffset + horizontalOffset * 2] = CellType.Stairs;
 
                             PlaceStairs(prev + horizontalOffset);
                             PlaceStairs(prev + horizontalOffset * 2);
                             PlaceStairs(prev + verticalOffset + horizontalOffset);
                             PlaceStairs(prev + verticalOffset + horizontalOffset * 2);
                         }
 
                         Debug.DrawLine(prev + new Vector3(0.5f, 0.5f, 0.5f), current + new Vector3(0.5f, 0.5f, 0.5f), Color.blue, 100, false);
                     }
                 }
 
                 foreach (var pos in path) {
                     if (grid[pos] == CellType.Hallway) {
                         PlaceHallway(pos);
                     }
                 }
             }
         }
     }
    void PlaceRoom(Vector3 location)
    {
        int randomIndex = 0;
        /*
        for (int i = 0; i < maxRoomCount * 2; i++)
        {
            randomIndex = random.Next(0, roomPrefabs.Length);
            if (roomPrefabMaxCount[randomIndex] > 0)
            {
                roomPrefabMaxCount[randomIndex] -= 1;
                break;
            }
        }
        */
        Instantiate(roomPrefabs[randomIndex], location, Quaternion.identity);
    }

    private void PlaceHallway(Vector3 location)
    {
        int randomIndex = random.Next(0, corridorPrefabs.Length);
        /*
        for (int i = 0; i < maxRoomCount * 2; i++)
        {
            randomIndex = random.Next(0, roomPrefabs.Length);
            if (roomPrefabMaxCount[randomIndex] > 0)
            {
                roomPrefabMaxCount[randomIndex] -= 1;
                break;
            }
        }
        */
        Instantiate(corridorPrefabs[randomIndex], location, Quaternion.identity);
    }

    private void PlaceStairs(Vector3 location)
    {
        Instantiate(corridorPrefabs[0], location, Quaternion.identity);
    }

    private static Vector3 GetPrefabSize(GameObject prefab)
    {
        Bounds totalBounds = new Bounds(prefab.transform.position, Vector3.zero);
        MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length > 0)
        {
            foreach (MeshRenderer renderer in renderers)
            {
                totalBounds.Encapsulate(renderer.bounds);   
            }
        }
        else
            Debug.LogWarning("Prefab does not have any MeshRenderer components.");

        return totalBounds.size;


    }
}
    
    
