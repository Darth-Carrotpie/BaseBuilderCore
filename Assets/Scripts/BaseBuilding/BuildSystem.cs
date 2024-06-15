using BovineLabs.Core.States;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[UpdateBefore(typeof(BuildOrderToPositionConsumerSystem))]
[BurstCompile]
public partial struct BuildSystem : ISystem
{
    EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildOrder>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        //dont run this, this system is old. Function moved to GridCellStateSetter

        state.Enabled = false;
        return;

        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        RefRW < BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
        //UnityEngine.Debug.Log("BuildSystem...");
        if (order.ValueRW.classValue == BuildingType.None) return;
        UnityEngine.Debug.Log("Build order received!");
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
            byte newState = OrderToState(order.ValueRW);
            UnityEngine.Debug.Log("new state in bytes: " + newState);
            //ecb.SetComponent(selectedEntity, new GridCellVisualState { Value = OrderToState(order.ValueRW) });
            RefRW<GridCellVisualState> cellState = entityManager.GetComponentObject<RefRW<GridCellVisualState>>(selectedEntity);
            cellState.ValueRW.Value = newState;

            //GridCellVisualState cellState = entityManager.GetComponentData<GridCellVisualState>(selectedEntity);
            //cellState.Value = newState;

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
        //this is not an order consumer, no need for this anymore:
        //order.ValueRW.classValue = BuildingType.None;

        //ecb.Playback(state.EntityManager); the WorldUnmanaged ECB will automatically Playback at the end of Update cycle
    }
    public Entity GetOderPrefab(BuildOrder order)
    {
        Entity output = Entity.Null;
        switch (order.classValue)
        {
            case BuildingType.Clear: output = order.cellPrefabEntityClear; break;
            case BuildingType.Workshop: output = order.cellPrefabEntityWorkshop; break;
            case BuildingType.Kitchen: output = order.cellPrefabEntityKitchen; break;
            case BuildingType.Barracks: output = order.cellPrefabEntityBarracks; break;
            case BuildingType.Arena: output = order.cellPrefabEntityArena; break;
        }
        return output;
    }
    private byte OrderToState(BuildOrder order)
    {
        byte output = 0;
        switch (order.classValue)
        {
            case BuildingType.Workshop: output = (byte)GridCellVisualStates.Workshop; break;
            case BuildingType.Kitchen: output = (byte)GridCellVisualStates.Kitchen; break;
            case BuildingType.Barracks: output = (byte)GridCellVisualStates.Barracks; break;
            case BuildingType.Arena: output = (byte)GridCellVisualStates.Arena; break;
            case BuildingType.Clear: output = (byte)GridCellVisualStates.Clear; break;
        }
        return output;
    }
}