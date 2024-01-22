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
            AddComponent(entity, new IsClearTag());
            SetComponentEnabled<IsClearTag>(entity, true);
            AddComponent(entity, new IsWorkshopTag());
            SetComponentEnabled<IsWorkshopTag>(entity, false);
            AddComponent(entity, new IsKitchenTag());
            SetComponentEnabled<IsKitchenTag>(entity, false);
            AddComponent(entity, new IsBarracksTag());
            SetComponentEnabled<IsBarracksTag>(entity, false);
            AddComponent(entity, new IsArenaTag());
            SetComponentEnabled<IsArenaTag>(entity, false);
        }
    }
}
public struct GridCell : IComponentData { }

public struct IsClearTag : IComponentData, IEnableableComponent{}
public struct IsWorkshopTag : IComponentData, IEnableableComponent{ }
public struct IsKitchenTag : IComponentData, IEnableableComponent{ }
public struct IsBarracksTag : IComponentData, IEnableableComponent{ }
public struct IsArenaTag : IComponentData, IEnableableComponent{ }
