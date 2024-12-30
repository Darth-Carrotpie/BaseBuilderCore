using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GridGeneratorConfigAuthoring : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject cellSelectorPrefab;
    public int gridSizeX;
    public int gridSizeZ;
    public HexOrientation hexOrientation;
    public float hexRadius;

    private class Baker : Baker<GridGeneratorConfigAuthoring>
    {
        public override void Bake(GridGeneratorConfigAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GridGeneratorConfig
            {
                cellPrefabEntity = GetEntity(authoring.cellPrefab, TransformUsageFlags.Dynamic),
                cellSelectorPrefabEntity = GetEntity(authoring.cellSelectorPrefab, TransformUsageFlags.Dynamic),
                gridSizeX = authoring.gridSizeX,
                gridSizeZ = authoring.gridSizeZ,
                hexOrientation = authoring.hexOrientation,
                hexRadius = authoring.hexRadius,
            });
        }
    }
}

public struct GridGeneratorConfig : IComponentData
{

    public Entity cellPrefabEntity;
    public Entity cellSelectorPrefabEntity;
    public int gridSizeX;
    public int gridSizeZ;
    public HexOrientation hexOrientation;
    public float hexRadius;
}
public enum HexOrientation
{
    Pointy,
    Flat
}