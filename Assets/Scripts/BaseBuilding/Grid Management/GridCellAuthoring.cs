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
            /*AddComponent(entity, new IsClearTag());
            SetComponentEnabled<IsClearTag>(entity, true);
            AddComponent(entity, new IsWorkshopTag());
            SetComponentEnabled<IsWorkshopTag>(entity, false);
            AddComponent(entity, new IsKitchenTag());
            SetComponentEnabled<IsKitchenTag>(entity, false);
            AddComponent(entity, new IsBarracksTag());
            SetComponentEnabled<IsBarracksTag>(entity, false);
            AddComponent(entity, new IsArenaTag());
            SetComponentEnabled<IsArenaTag>(entity, false);*/
            AddBuffer<GridCellArea>(entity);

            int clearState = 254;
            AddComponent(entity, new GridCellVisualState());
            SetComponent(entity, new GridCellVisualState { Value = (byte)clearState });
            AddComponent(entity, new GridCellVisualStatePrevious { Value = (byte)clearState });

            AddComponent(entity, new ClearGridCellVisualState()); 
            //SetComponentEnabled<ClearGridCellVisualState>(entity, false) ;
            AddComponent(entity, new ArenaGridCellVisualState());
            //SetComponentEnabled<ArenaGridCellVisualState>(entity, false);
            AddComponent(entity, new WorkshopGridCellVisualState());
            //SetComponentEnabled<WorkshopGridCellVisualState>(entity, false);
            AddComponent(entity, new KitchenGridCellVisualState());
            //SetComponentEnabled<KitchenGridCellVisualState>(entity, false);
            AddComponent(entity, new BarracksGridCellVisualState());
            //SetComponentEnabled<BarracksGridCellVisualState>(entity, false); 
        }
    }
}
public struct GridCell : IComponentData {
    public Entity cellUI;
    public Entity building;
}
