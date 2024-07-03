using BovineLabs.Core;
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
//[BurstCompile]
public partial struct HexVisualsSystem : ISystem
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
        //state.Enabled = false;
        //return;

        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error

        foreach ((var vState, var gridCell, var prevState, var currState, Entity selectedEntity) in SystemAPI.Query<ArenaGridCellVisualState, RefRW<GridCell>, GridCellVisualStatePrevious, GridCellVisualState>().WithEntityAccess())
        {
            if (prevState.Value == currState.Value) return;
            //UnityEngine.Debug.Log("State changed for at least one ArenaGridCellVisualState!");

            Entity toBuild = order.ValueRW.cellPrefabEntityArena;
            InstantiateVisuals(toBuild, ecb, selectedEntity, gridCell);
        }
        foreach ((var vState, var gridCell, var prevState, var currState, Entity selectedEntity) in SystemAPI.Query<ClearGridCellVisualState, RefRW<GridCell>, GridCellVisualStatePrevious, GridCellVisualState>().WithEntityAccess())
        {
            if (prevState.Value == currState.Value) return;
            //UnityEngine.Debug.Log("State changed for at least one ClearGridCellVisualState!");

            Entity toBuild = order.ValueRW.cellPrefabEntityClear;
            InstantiateVisuals(toBuild, ecb, selectedEntity, gridCell);
        }
        foreach ((var vState, var gridCell, var prevState, var currState, Entity selectedEntity) in SystemAPI.Query<WorkshopGridCellVisualState, RefRW<GridCell>, GridCellVisualStatePrevious, GridCellVisualState>().WithEntityAccess())
        {
            if (prevState.Value == currState.Value) return;
            //UnityEngine.Debug.Log("State changed for at least one ClearGridCellVisualState!");

            Entity toBuild = order.ValueRW.cellPrefabEntityWorkshop;
            InstantiateVisuals(toBuild, ecb, selectedEntity, gridCell);
        }
    }
    public void InstantiateVisuals(Entity toBuild, EntityCommandBuffer ecb, Entity selectedEntity, RefRW<GridCell> gridCell)
    {
        if (gridCell.ValueRW.cellUI != Entity.Null)
        {
            ecb.AddComponent(gridCell.ValueRW.cellUI, new MarkedForDestruction { }); //this will tag to destroy entity later
        }

        var newVisualModel = ecb.Instantiate(toBuild);
        ecb.AddComponent<Parent>(newVisualModel);
        ecb.SetComponent(newVisualModel, new LocalTransform
        {
            Position = float3.zero,
            Rotation = quaternion.identity,
            Scale = 1f
        });

        ecb.SetComponent(newVisualModel, new Parent { Value = selectedEntity });

        //UnityEngine.Debug.Log("setting new :" + newVisualModel);

        ecb.SetComponent(selectedEntity, new GridCell { cellUI = newVisualModel }); //this is not working!!!! WHYY!!!
        //string hexName = World.DefaultGameObjectInjectionWorld.EntityManager.GetName(selectedEntity);
        //UnityEngine.Debug.Log("Built a {" + order.ValueRW.classValue + "} at: " + hexName);

        //ecb.SetName(selectionUI, "Selector_of_"+selectedEntity);
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
}