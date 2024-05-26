using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class GridCellAuthoring : MonoBehaviour
{
    public GameObject unbuiltPrefab;
    public class Baker : Baker<GridCellAuthoring>
    {
        public override void Bake(GridCellAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GridCell
            {
                cellUI = GetEntity(authoring.unbuiltPrefab, TransformUsageFlags.Dynamic),
            });
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
            AddBuffer<NeighbourBuilding>(entity);
        }
    }
}
public struct GridCell : IComponentData {
    public Entity cellUI;
    public Entity building;
}

public struct IsClearTag : IComponentData, IEnableableComponent{
    public Entity cellUI;
}
public struct IsWorkshopTag : IComponentData, IEnableableComponent{
    public Entity cellUI;
}
public struct IsKitchenTag : IComponentData, IEnableableComponent{
    public Entity cellUI;
}
public struct IsBarracksTag : IComponentData, IEnableableComponent{
    public Entity cellUI;
}
public struct IsArenaTag : IComponentData, IEnableableComponent{
    public Entity cellUI;
}
