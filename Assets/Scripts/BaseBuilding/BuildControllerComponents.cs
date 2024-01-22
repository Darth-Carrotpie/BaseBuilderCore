using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BuildOrder : IComponentData
{
    public BuildingClass Value;
}


public struct BlockClickThrough : IComponentData
{
    public bool Value;
}