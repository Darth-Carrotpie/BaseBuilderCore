using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BuildOrder : IComponentData
{
    public BuildingType classValue;
    public Entity cellPrefabEntityClear;
    public Entity cellPrefabEntityWorkshop;
    public Entity cellPrefabEntityKitchen;
    public Entity cellPrefabEntityBarracks;
    public Entity cellPrefabEntityArena;
}


public struct BlockClickThrough : IComponentData
{
    public bool Value;
}

public struct BuildOrderAtPosition : IBufferElementData
{
    public bool isFirst;
    public BuildOrder buildOrder;
    public float3 position;
    public Entity buildingProduced;
    public Entity forceNodeProduced;
    public Entity forceLinkProduced;
}
