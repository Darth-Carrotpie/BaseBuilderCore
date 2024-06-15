using BovineLabs.Core.States;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(ConsumerSystemsGroup))]
public partial struct BuildOrderToPositionConsumerSystem : ISystem, ISystemStartStop
{
    private StateModel impl;
    EntityManager entityManager;

    [BurstCompile]
    public void OnStartRunning(ref SystemState state)
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    [BurstCompile]
    public void OnStopRunning(ref SystemState state)
    {
    }
     
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();

        DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);
        if (buildOrdersAtPos.Length <= 0) return;

        for (int i = buildOrdersAtPos.Length-1; i>=0; i--)
        {
            //remove element from the DynamicBuffer if both conditions are met, thus completing the build cycle:
            if (buildOrdersAtPos[i].buildingProduced != Entity.Null && 
                buildOrdersAtPos[i].forceNodeProduced != Entity.Null)
            {
                var bp = entityManager.GetName(buildOrdersAtPos[i].buildingProduced);
                var fp = entityManager.GetName(buildOrdersAtPos[i].forceNodeProduced);
                UnityEngine.Debug.Log("Consumer buildingProduced: " + bp+ "  forceNodeProduced:" + fp);
                buildOrdersAtPos.RemoveAt(i); 
            }
        }

        order.ValueRW.classValue = BuildingType.None;
    }
}