using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Vertex = Graphs.Vertex;

public class Generation : MonoBehaviour
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
        public Bounds bounds;
        public Vector3 location;

        public Room(Vector3 location, GameObject prefab)
        {
            //bounds = new Bounds(location, );
            this.location = location;
        }

        public static bool Intersect(Room a, Room b)
        {
            return
                a.bounds.Intersects(b.bounds);
            /*!((a.bounds. a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
            || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
            || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));*/
        }
    }

    [SerializeField] Vector3 size;
    [SerializeField] int roomCount;
    [SerializeField] Vector3 roomMaxSize;
    [SerializeField] Transform startExit;
    [SerializeField] Transform[] fireExitsSpawns;
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] GameObject[] corridorPrefabs;
    [SerializeField] GameObject cubePrefab;
    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;
    [SerializeField] Material greenMaterial;

    Random random;
    Grid3D<CellType> grid;
    List<Room> rooms;
    Delaunay delaunay;
    private HashSet<Prim.Edge> selectedEdges;
    private Vector3 corridorSize;

    void Start()
    {
        random = new Random();
        grid = new Grid3D<CellType>(size, Vector3.zero);
        rooms = new List<Room>();
        // corridorSize = GetPrefabSize(corridorPrefabs[0]);
        PlaceRooms();
        //Triangulate();
        //CreateHallways();
        //PathfindHallways();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            SceneManager.LoadScene(SceneManager.sceneCount);
        }
    }

    void PlaceExits()
    {
        // PlaceCube(new Vector3Int(startExit.position.x));
    }

    void PlaceRooms()
    {
        int count = 0;
        for (int i = 0; i < roomCount; i++)
        {
            Vector3 location = new Vector3(
                random.Next(0, (int)size.x),
                random.Next(0, (int)size.y),
                random.Next(0, (int)size.z)
            );
            GameObject roomPref = roomPrefabs[random.Next(0, roomPrefabs.Length)];
            bool add = true;
            Room newRoom = new Room(location, roomPref);
            // Room buffer = new Room(location + new Vector3(-1, 0, -1), //roomSize + new Vector3(2, 0, 2));

            /* foreach (var room in rooms) {
                 if (Room.Intersect(room, buffer)) {
                     add = false;
                     break;
                 }
             }
 
             if (newRoom.bounds.min.x< 0 || newRoom.bounds.max.x >= size.x
                 || newRoom.bounds.min.y < 0 || newRoom.bounds.max.y >= size.y
                 || newRoom.bounds.min.z < 0 || newRoom.bounds.max.z >= size.z) {
                 add = false;
             }
 
             if (add) {
                 Debug.Log(++count);
                 rooms.Add(newRoom);
                 PlaceRoom(newRoom.location, newRoom.bounds.size);
 
                 /*foreach (var pos in newRoom.bounds.allPositionsWithin) {
                     grid[pos] = CellType.Room;
                 }
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
 
     void PlaceCube(Vector3 location, Vector3 size, Material material) {
         GameObject go = Instantiate(cubePrefab, location, Quaternion.identity);
         go.GetComponent<Transform>().localScale = size;
         go.GetComponent<MeshRenderer>().material = material;
     }
 
     void PlaceRoom(Vector3 location, GameObject room)
     {
         Instantiate(room, location, Quaternion.identity);
     }
 
     void PlaceHallway(Vector3 location) {
         //PlaceCube(location, new Vector3Int(1, 1, 1), blueMaterial);
         Instantiate(corridorPrefabs[0], location, Quaternion.identity);
     }
 
     void PlaceStairs(Vector3 location) {
         PlaceCube(location, new Vector3(1, 1, 1), greenMaterial);
     }
     
     Vector3 GetPrefabSize(GameObject prefab) {
         Bounds totalBounds = new Bounds(prefab.transform.position, Vector3.zero);
         MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>();
 
         if (renderers.Length > 0) {
             foreach (MeshRenderer renderer in renderers) {
                 totalBounds.Encapsulate(renderer.bounds);
             }
         } else {
             Debug.LogWarning("Prefab does not have any MeshRenderer components.");
         }
 
         Debug.Log(totalBounds.size);
         return totalBounds.size;
     }
 */

        }
    }
}