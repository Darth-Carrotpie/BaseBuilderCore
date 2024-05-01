using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct BuildSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildOrder>();
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        RefRW < BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
        if (order.ValueRW.classValue == BuildingClass.None) return;

        foreach ((RefRW<SelectableCellTag> cell, RefRW<GridCell> gridCell, Entity selectedEntity) in SystemAPI.Query<RefRW<SelectableCellTag>, RefRW<GridCell>>().WithAll<SelectedCellTag>().WithEntityAccess())
        {
            Entity toBuild = GetOderPrefab(order.ValueRW);
            var building = ecb.Instantiate(toBuild);
            ecb.AddComponent<Parent>(building);
            ecb.SetComponent(building, new LocalTransform
            {
                Position = float3.zero,
                Rotation = quaternion.identity,
                Scale = 1f
            });
            ecb.SetComponent(building, new Parent { Value = selectedEntity });
            ecb.SetComponentEnabled<SelectedCellTag>(selectedEntity, false);
            if (gridCell.ValueRW.cellUI != Entity.Null)
            {
                ecb.DestroyEntity(gridCell.ValueRW.cellUI); //this makes cellUI invalid, OK!
            }
            //gridCell.ValueRW.cellUI = Entity.Null;
            ecb.SetComponent(selectedEntity, new GridCell { cellUI = building }); //this is not working!!!! WHYY!!!
            string hexName = World.DefaultGameObjectInjectionWorld.EntityManager.GetName(selectedEntity);
            UnityEngine.Debug.Log("Built a {"+ order.ValueRW.classValue + "} at: "+ hexName);

            //ecb.SetName(selectionUI, "Selector_of_"+selectedEntity);
        }
        order.ValueRW.classValue = BuildingClass.None;
    }
    public Entity GetOderPrefab(BuildOrder order)
    {
        Entity output = Entity.Null;
        switch (order.classValue)
        {
            case BuildingClass.Clear: output = order.cellPrefabEntityClear; break;
            case BuildingClass.Workshop: output = order.cellPrefabEntityWorkshop; break;
            case BuildingClass.Kitchen: output = order.cellPrefabEntityKitchen; break;
            case BuildingClass.Barracks: output = order.cellPrefabEntityBarracks; break;
            case BuildingClass.Arena: output = order.cellPrefabEntityArena; break;
        }
        return output;
    }
}