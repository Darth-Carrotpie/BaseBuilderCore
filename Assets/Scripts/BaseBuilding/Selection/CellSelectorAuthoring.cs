using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class CellSelectorAuthoring : MonoBehaviour
{
    public class Baker : Baker<CellSelectorAuthoring>
    {
        public override void Bake(CellSelectorAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CellSelector());
        }
    }
}
public struct CellSelector : IComponentData { }
