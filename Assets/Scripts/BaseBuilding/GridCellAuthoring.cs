using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class GridCellAuthoring : MonoBehaviour
{
    public class Baker : Baker<GridCellAuthoring>
    {
        public override void Bake(GridCellAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GridCell());
            AddComponent(entity, new SelectableCellTag());
            SetComponentEnabled<SelectableCellTag>(entity, true);
            AddComponent(entity, new SelectedCellTag());
            SetComponentEnabled<SelectedCellTag>(entity, false);
        }
    }
}
public struct GridCell : IComponentData { }
