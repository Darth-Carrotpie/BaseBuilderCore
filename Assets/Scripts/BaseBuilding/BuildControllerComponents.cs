using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BuildOrder : IComponentData
{
    public BuildingClass classValue;
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