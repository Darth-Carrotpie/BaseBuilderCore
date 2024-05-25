using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BuildControllerAuthoring : MonoBehaviour
{
    public GameObject cellPrefabEntityClear;
    public GameObject cellPrefabEntityWorkshop;
    public GameObject cellPrefabEntityKitchen;
    public GameObject cellPrefabEntityBarracks;
    public GameObject cellPrefabEntityArena;

    public class Baker : Baker<BuildControllerAuthoring>
    {
        public override void Bake(BuildControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BuildOrder {
                cellPrefabEntityClear = GetEntity(authoring.cellPrefabEntityClear, TransformUsageFlags.Dynamic),
                cellPrefabEntityWorkshop = GetEntity(authoring.cellPrefabEntityWorkshop, TransformUsageFlags.Dynamic),
                cellPrefabEntityKitchen = GetEntity(authoring.cellPrefabEntityKitchen, TransformUsageFlags.Dynamic),
                cellPrefabEntityBarracks = GetEntity(authoring.cellPrefabEntityBarracks, TransformUsageFlags.Dynamic),
                cellPrefabEntityArena = GetEntity(authoring.cellPrefabEntityArena, TransformUsageFlags.Dynamic),
            });
            AddComponent(entity, new BlockClickThrough());
            AddComponent(entity, new ExcludeFromAutoParenting());
        }
    }
}
