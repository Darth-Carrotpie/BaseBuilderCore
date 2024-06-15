using BovineLabs.Core.States;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[UpdateBefore(typeof(BuildOrderToPositionConsumerSystem))]
[BurstCompile]
public partial struct GridCellStateSetter : ISystem
{
    EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ForceNode>();
        state.RequireForUpdate<GridCell>();
        //Registering injects a query filter to the system State. This can act similar to RequireForUpdate if argument is true
        StateAPI.Register<GridCellVisualState, ArenaGridCellVisualState>(ref state, (byte)GridCellVisualStates.Arena, false);
        StateAPI.Register<GridCellVisualState, WorkshopGridCellVisualState>(ref state, (byte)GridCellVisualStates.Workshop, false);
        StateAPI.Register<GridCellVisualState, BarracksGridCellVisualState>(ref state, (byte)GridCellVisualStates.Barracks, false);
        StateAPI.Register<GridCellVisualState, KitchenGridCellVisualState>(ref state, (byte)GridCellVisualStates.Kitchen, false);
        StateAPI.Register<GridCellVisualState, ClearGridCellVisualState>(ref state, (byte)GridCellVisualStates.Clear, false);
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

        //track cells which have a ForceNode relation:
        var stateFullCells = new NativeList<Entity>(Allocator.Temp);

        //if (order.ValueRW.classValue == BuildingType.None) return;
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);


        //Set new state:
        //query all forceNodes
        foreach ((ForceNode forceNode, DynamicBuffer<GridCellArea> dynBuffer) in SystemAPI.Query<ForceNode, DynamicBuffer<GridCellArea>>())
        {
            if (forceNode.buildingRepr == Entity.Null) continue;
        //check their ref to current grid cell
            Entity gridCellEntity = Entity.Null;
            foreach (var cellEntity in dynBuffer)
            {
                if(gridCellEntity == Entity.Null)
                {
                    GridCellVisualState currState = entityManager.GetComponentData<GridCellVisualState>(cellEntity.GridCellEntity);
                    if (currState.Value == (byte)GridCellVisualStates.Clear)
                    {
                        gridCellEntity = cellEntity.GridCellEntity;
                        stateFullCells.Add(gridCellEntity);
                    }
                }
            }
            if(gridCellEntity == Entity.Null) continue; 
            //set state of the grid cell to appropriate state of what the buildin of that force node
            Building building = entityManager.GetComponentData<Building>(forceNode.buildingRepr);
            byte newState = BuildingToCellState(building.buildingType);
            ecb.SetComponent(gridCellEntity, new GridCellVisualState { Value = newState });
            //UnityEngine.Debug.Log(entityManager.GetName(gridCellEntity)+ " <new state in bytes>: " + newState);
        }
        //clear all cells from their state which were not paired with a node
        foreach ((RefRW<GridCell> gridCell, Entity cellEntity) in SystemAPI.Query<RefRW<GridCell>>().WithEntityAccess())
        {
            if (!stateFullCells.Contains(cellEntity))
            {
                //ecb.SetComponent(cellEntity, new GridCellVisualState { Value = (byte)GridCellVisualStates.Clear });
                ecb.SetComponent(cellEntity, new GridCellVisualState { Value = BuildingToCellState(BuildingType.Clear) });
                //UnityEngine.Debug.Log(entityManager.GetName(cellEntity) + " <new state in bytes>: Clear");
            }
        }
        stateFullCells.Dispose();
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
    private byte BuildingToCellState(BuildingType buildingType)
    {
        byte output = 0;
        switch (buildingType)
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