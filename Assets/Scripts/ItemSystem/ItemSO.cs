using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Properties")] 
    public Sprite itemSprite;
    public string itemName;
    public Vector3 itemPositionOffset;
    public Quaternion itemRotationOffset;
    public ItemType type;
    
    [Header("IK position (skip if dont need)")]
    public Vector3 rightHandPosition;
    public Quaternion rightHandRotation;
}

public enum ItemType
{
    Melee,
    Simple,
    Gun
}

