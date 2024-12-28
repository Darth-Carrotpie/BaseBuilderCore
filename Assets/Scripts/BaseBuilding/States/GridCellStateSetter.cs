using BovineLabs.Core.Spatial;
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
        //state.RequireForUpdate<ForceNode>(); //there can be an empty map after last node is destroyed. Do not require ForceNode otherwise visuals will not update!
        state.RequireForUpdate<GridCell>();
        //can use the StateAPI.Register just once per system. It's meant to use a single state for a single system.
        //Registering injects a query filter to the system State. This can act similar to RequireForUpdate if argument is true
        //StateAPI.Register<GridCellVisualState, ArenaGridCellVisualState>(ref state, (byte)GridCellVisualStates.Arena, false);
        //StateAPI.Register<GridCellVisualState, WorkshopGridCellVisualState>(ref state, (byte)GridCellVisualStates.Workshop, false);
        //StateAPI.Register<GridCellVisualState, BarracksGridCellVisualState>(ref state, (byte)GridCellVisualStates.Barracks, false);
        //StateAPI.Register<GridCellVisualState, KitchenGridCellVisualState>(ref state, (byte)GridCellVisualStates.Kitchen, false);
        //StateAPI.Register<GridCellVisualState, ClearGridCellVisualState>(ref state, (byte)GridCellVisualStates.Clear, false);
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        RegisterMultipleState(ref state);
    }
    void RegisterMultipleState(ref SystemState state)
    {
        var stateTypeIndex = TypeManager.GetTypeIndex<GridCellVisualState>();

        var e0 = state.EntityManager.CreateEntity(typeof(StateInstance));
        entityManager.SetComponentData(e0, new StateInstance
        {
            State = stateTypeIndex,
            StateKey = (byte)GridCellVisualStates.Clear,
            StateInstanceComponent = TypeManager.GetTypeIndex<ClearGridCellVisualState>(),
        });

        var e1 = state.EntityManager.CreateEntity(typeof(StateInstance));
        entityManager.SetComponentData(e1, new StateInstance
        {
            State = stateTypeIndex,
            StateKey = (byte)GridCellVisualStates.Arena,
            StateInstanceComponent = TypeManager.GetTypeIndex<ArenaGridCellVisualState>(),
        });
        var e2 = state.EntityManager.CreateEntity(typeof(StateInstance));
        entityManager.SetComponentData(e2, new StateInstance
        {
            State = stateTypeIndex,
            StateKey = (byte)GridCellVisualStates.Workshop,
            StateInstanceComponent = TypeManager.GetTypeIndex<WorkshopGridCellVisualState>(),
        });
        var e3 = state.EntityManager.CreateEntity(typeof(StateInstance));
        entityManager.SetComponentData(e3, new StateInstance
        {
            State = stateTypeIndex,
            StateKey = (byte)GridCellVisualStates.Barracks,
            StateInstanceComponent = TypeManager.GetTypeIndex<BarracksGridCellVisualState>(),
        });
        var e4 = state.EntityManager.CreateEntity(typeof(StateInstance));
        entityManager.SetComponentData(e4, new StateInstance
        {
            State = stateTypeIndex,
            StateKey = (byte)GridCellVisualStates.Kitchen,
            StateInstanceComponent = TypeManager.GetTypeIndex<KitchenGridCellVisualState>(),
        });

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
            GridCellArea gca = dynBuffer.AsNativeArray().FirstOrDefault();
            gridCellEntity = gca.GridCellEntity;

            /*foreach (var bufferElement in dynBuffer)
            {
                if (gridCellEntity == Entity.Null)
                {
                    GridCellVisualState currState = entityManager.GetComponentData<GridCellVisualState>(bufferElement.GridCellEntity);
                    //GridCellVisualStatePrevious prevState = entityManager.GetComponentData<GridCellVisualStatePrevious>(cellEntity.GridCellEntity);
                    if (currState.Value == (byte)GridCellVisualStates.Clear)
                    {
                        gridCellEntity = bufferElement.GridCellEntity;
                    }
                    stateFullCells.Add(bufferElement.GridCellEntity);
                }
            }*/
            if (gridCellEntity == Entity.Null) continue;
            stateFullCells.Add(gridCellEntity);

            //set state of the grid cell to appropriate state of what the buildin of that force node
            GridCellVisualState currState = entityManager.GetComponentData<GridCellVisualState>(gridCellEntity);
            GridCellVisualStatePrevious prevState = entityManager.GetComponentData<GridCellVisualStatePrevious>(gridCellEntity);
            Building building = entityManager.GetComponentData<Building>(forceNode.buildingRepr);
            byte newState = BuildingToCellState(building.buildingType);

            //somehow no matter how I try to change the state, it won't apply the change onto it.
            //each frame I try this and each frame it fails silently. Try/catch?
            if (currState.Value != newState)
            {
                //currState.Value = newState;
                //this--------------------------------
                string entName = entityManager.GetName(gridCellEntity);
                //UnityEngine.Debug.Log(entName+" >>> prevState:"+ prevState.Value + " currState: " + currState.Value + "  newState:" + newState );
                ecb.AddComponent(gridCellEntity, new GridCellVisualState { Value = newState });
                //------------------------------
                //UnityEngine.Debug.Log(entityManager.GetName(gridCellEntity)+ " <new state in bytes>: " + newState);
            }
        }
        //clear all cells from their state which were not paired with a node
        //if this is slow, could make it into a burstable job
        foreach ((RefRW<GridCell> gridCell, Entity cellEntity) in SystemAPI.Query<RefRW<GridCell>>().WithNone<ClearGridCellVisualState>().WithEntityAccess())
        {
            if (!stateFullCells.Contains(cellEntity)) {
                //ecb.SetComponent(cellEntity, new GridCellVisualState { Value = (byte)GridCellVisualStates.Clear });
                ecb.SetComponent(cellEntity, new GridCellVisualState { Value = BuildingToCellState(BuildingType.Clear) });
                //UnityEngine.Debug.Log(entityManager.GetName(cellEntity) + " <new state in bytes>: Clear");
                //UnityEngine.Debug.Log("setting new state:" + BuildingType.Clear);
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