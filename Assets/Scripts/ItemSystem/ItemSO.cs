using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Properties")] 
    public string itemID;
    public Sprite itemSprite;
    public string itemName;
    public GameObject itemPrefab;
    public Vector3 itemPositionOffset;
}

