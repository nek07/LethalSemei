using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D<T> {
    T[] data;

    public Vector3 Size { get; private set; }
    public Vector3 Offset { get; set; }

    public Grid3D(Vector3 size, Vector3 offset) {
        Size = size;
        Offset = offset;

        data = new T[(int)(size.x * size.y * size.z)];
    }

    private int GetIndex(Vector3 pos) {
        return (int)(pos.x + (Size.x * pos.y) + (Size.x * Size.y * pos.z));
    }

    public bool InBounds(Vector3 pos) {
        return new Bounds(Vector3.zero, Size).Contains(pos + Offset);
    }

    public T this[int x, int y, int z] {
        get {
            return this[new Vector3(x, y, z)];
        }
        set {
            this[new Vector3(x, y, z)] = value;
        }
    }

    public T this[Vector3 pos] {
        get {
            pos += Offset;
            return data[GetIndex(pos)];
        }
        set {
            pos += Offset;
            data[GetIndex(pos)] = value;
        }
    }
}