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
[UpdateBefore(typeof(GridCellStateSetterTest))]
[UpdateBefore(typeof(GridCellStateSetter))]
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
        RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>();


        foreach ((var prevState, var currState, var gridCell, Entity stateChangeEntity) in SystemAPI.Query<GridCellVisualStatePrevious, GridCellVisualState, RefRW<GridCell>>().WithEntityAccess())
        {
            if (prevState.Value != currState.Value)
            {
                Entity toBuild = StateByteToPrefab(currState.Value, order);
                InstantiateVisuals(toBuild, ecb, stateChangeEntity, gridCell);

                string entName = entityManager.GetName(stateChangeEntity);
                //UnityEngine.Debug.Log("RefRW+State!>> Changed: " + entName + ">>>  prevState:" + prevState.Value + " currState: " + currState.Value);
            }
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

    public Entity StateByteToPrefab(byte stateByte, RefRW<BuildOrder> order)
    {
        Entity output = Entity.Null;
        switch (stateByte)
        {
            case (byte)GridCellVisualStates.Clear: output = order.ValueRW.cellPrefabEntityClear; break;
            case (byte)GridCellVisualStates.Workshop: output = order.ValueRW.cellPrefabEntityWorkshop; break;
            case (byte)GridCellVisualStates.Kitchen: output = order.ValueRW.cellPrefabEntityKitchen; break;
            case (byte)GridCellVisualStates.Barracks: output = order.ValueRW.cellPrefabEntityBarracks; break;
            case (byte)GridCellVisualStates.Arena: output = order.ValueRW.cellPrefabEntityArena; break;
        }
        return output;
    }
}