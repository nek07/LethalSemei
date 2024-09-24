using System;
using System.Collections.Generic;
using UnityEngine;
using BlueRaja;

public class PathFinder {
    //similar to A*, with some modifications specifically for stair cases
    public class Node {
        public Vector3 Position { get; private set; }
        public Node Previous { get; set; }
        public HashSet<Vector3> PreviousSet { get; private set; }
        public float Cost { get; set; }

        public Node(Vector3 position) {
            Position = position;
            PreviousSet = new HashSet<Vector3>();
        }
    }

    public struct PathCost {
        public bool traversable;
        public float cost;
        public bool isStairs;
    }

    static readonly Vector3[] neighbors = {
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),

        new Vector3(3, 1, 0),
        new Vector3(-3, 1, 0),
        new Vector3(0, 1, 3),
        new Vector3(0, 1, -3),

        new Vector3(3, -1, 0),
        new Vector3(-3, -1, 0),
        new Vector3(0, -1, 3),
        new Vector3(0, -1, -3),
    };

    Grid3D<Node> grid;
    SimplePriorityQueue<Node, float> queue;
    HashSet<Node> closed;
    Stack<Vector3> stack;

    public PathFinder(Vector3 size) {
        grid = new Grid3D<Node>(size, Vector3.zero);

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();
        stack = new Stack<Vector3>();

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    grid[x, y, z] = new Node(new Vector3(x, y, z));
                }
            }
        }
    }

    void ResetNodes() {
        var size = grid.Size;

        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                for (int z = 0; z < size.z; z++) {
                    var node = grid[x, y, z];
                    node.Previous = null;
                    node.Cost = float.PositiveInfinity;
                    node.PreviousSet.Clear();
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end, Func<Node, Node, PathCost> costFunction) {
        ResetNodes();
        queue.Clear();
        closed.Clear();

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();

        grid[start].Cost = 0;
        queue.Enqueue(grid[start], 0);

        while (queue.Count > 0) {
            Node node = queue.Dequeue();
            closed.Add(node);

            if (node.Position == end) {
                return ReconstructPath(node);
            }

            foreach (var offset in neighbors) {
                if (!grid.InBounds(node.Position + offset)) continue;
                var neighbor = grid[node.Position + offset];
                if (closed.Contains(neighbor)) continue;

                if (node.PreviousSet.Contains(neighbor.Position)) {
                    continue;
                }

                var pathCost = costFunction(node, neighbor);
                if (!pathCost.traversable) continue;

                if (pathCost.isStairs) {
                    float xDir = Mathf.Clamp(offset.x, -1, 1);
                    float zDir = Mathf.Clamp(offset.z, -1, 1);
                    Vector3 verticalOffset = new Vector3(0, offset.y, 0);
                    Vector3 horizontalOffset = new Vector3(xDir, 0, zDir);

                    if (node.PreviousSet.Contains(node.Position + horizontalOffset)
                        || node.PreviousSet.Contains(node.Position + horizontalOffset * 2)
                        || node.PreviousSet.Contains(node.Position + verticalOffset + horizontalOffset)
                        || node.PreviousSet.Contains(node.Position + verticalOffset + horizontalOffset * 2)) {
                        continue;
                    }
                }

                float newCost = node.Cost + pathCost.cost;

                if (newCost < neighbor.Cost) {
                    neighbor.Previous = node;
                    neighbor.Cost = newCost;

                    if (queue.TryGetPriority(node, out float existingPriority)) {
                        queue.UpdatePriority(node, newCost);
                    } else {
                        queue.Enqueue(neighbor, neighbor.Cost);
                    }

                    neighbor.PreviousSet.Clear();
                    neighbor.PreviousSet.UnionWith(node.PreviousSet);
                    neighbor.PreviousSet.Add(node.Position);

                    if (pathCost.isStairs){
                        float xDir = Mathf.Clamp(offset.x, -1, 1);
                        float zDir = Mathf.Clamp(offset.z, -1, 1);
                        Vector3 verticalOffset = new Vector3(0, offset.y, 0);
                        Vector3 horizontalOffset = new Vector3(xDir, 0, zDir);

                        neighbor.PreviousSet.Add(node.Position + horizontalOffset);
                        neighbor.PreviousSet.Add(node.Position + horizontalOffset * 2);
                        neighbor.PreviousSet.Add(node.Position + verticalOffset + horizontalOffset);
                        neighbor.PreviousSet.Add(node.Position + verticalOffset + horizontalOffset * 2);
                    }
                }
            }
        }

        return null;
    }

    List<Vector3> ReconstructPath(Node node) {
        List<Vector3> result = new List<Vector3>();

        while (node != null) {
            stack.Push(node.Position);
            node = node.Previous;
        }

        while (stack.Count > 0) {
            result.Add(stack.Pop());
        }

        return result;
    }
}