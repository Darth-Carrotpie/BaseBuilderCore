using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BuildingAuthoring : MonoBehaviour
{

    public class Baker : Baker<BuildControllerAuthoring>
    {
        public override void Bake(BuildControllerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Building());
        }
    }
}
public struct Building : IComponentData
{
    public BuildingType buildingType;
}