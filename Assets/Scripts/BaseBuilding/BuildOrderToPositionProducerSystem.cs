using BovineLabs.Core.States;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ProducerSystemsGroup))]
public partial struct BuildOrderToPositionProducerSystem : ISystem, ISystemStartStop
{
    private StateModel impl;
    EntityManager entityManager;

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        this.impl = new StateModel(ref state, ComponentType.ReadWrite<GridCellVisualState>(), ComponentType.ReadWrite<GridCellVisualStatePrevious>());
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
        this.impl.Dispose(ref state);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
        if (order.ValueRW.classValue == BuildingType.None) return;

        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity(); 

        DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);

        foreach ((var localTransform,var cell, var gridCell, Entity gridCellEntity) in SystemAPI.Query<LocalTransform, SelectableCellTag, GridCell>().WithAll<SelectedCellTag>().WithEntityAccess())
        {
            BuildOrderAtPosition newBOatPosition = new BuildOrderAtPosition
            {
                buildOrder = order.ValueRW,
                position = localTransform.Position,
                buildingProduced = Entity.Null,
                forceNodeProduced = Entity.Null,
            };
            buildOrdersAtPos.Add(newBOatPosition);
            Debug.Log("pos:" + localTransform.Position);
            entityManager.SetComponentEnabled<SelectedCellTag>(gridCellEntity, false);
        }
        order.ValueRW.classValue = BuildingType.None;
    }
}