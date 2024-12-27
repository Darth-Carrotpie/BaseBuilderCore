using BovineLabs.Core.States;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
    public void OnStopRunning(ref SystemState state)
    {
    }
     
    //[BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();
        DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);

        //consume Markers which help identify data for build orders:
        if (buildOrdersAtPos.Length <= 0)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var (marker, entity) in SystemAPI.Query<MarkedNodeForLinkStart>().WithEntityAccess())
            {
                ecb.RemoveComponent<MarkedNodeForLinkStart>(entity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            return;
        }

        for (int i = buildOrdersAtPos.Length-1; i>=0; i--) {
            //remove element from the DynamicBuffer if both conditions are met, thus completing the build cycle: 
            if (buildOrdersAtPos[i].buildingProduced != Entity.Null && 
                buildOrdersAtPos[i].forceNodeProduced != Entity.Null &&
                (buildOrdersAtPos[i].forceLinkProduced != Entity.Null ||
                entityManager.HasComponent<MarkedNodeForLinkStart>(buildOrdersAtPos[i].forceNodeProduced)
                ) )
            {
                var bp = entityManager.GetName(buildOrdersAtPos[i].buildingProduced);
                var fp = entityManager.GetName(buildOrdersAtPos[i].forceNodeProduced);
                UnityEngine.Debug.Log("Consumed buildOrdersAtPos: " + bp+ "  forceNodeProduced:" + fp);
                buildOrdersAtPos.RemoveAt(i);
                continue;
            }
            //if there is just a single node, there won't be anything to link to, so we remove the order
            var nodesQuery = entityManager.CreateEntityQuery(typeof(ForceNode)).ToEntityArray(Allocator.TempJob);
            if (nodesQuery.Length <= 1)
            {
                if (buildOrdersAtPos[i].buildingProduced != Entity.Null &&
                    buildOrdersAtPos[i].forceNodeProduced != Entity.Null)
                {
                    var bp = entityManager.GetName(buildOrdersAtPos[i].buildingProduced);
                    var fp = entityManager.GetName(buildOrdersAtPos[i].forceNodeProduced);
                    UnityEngine.Debug.Log("Consumed buildOrdersAtPos: " + bp + "  forceNodeProduced:" + fp);
                    buildOrdersAtPos.RemoveAt(i);
                }
            }

        }
        if (buildOrdersAtPos.Length <= 0)
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            foreach (var (marker, entity) in SystemAPI.Query<MarkedNodeForLinkStart>().WithEntityAccess())
            {
                ecb.RemoveComponent<MarkedNodeForLinkStart>(entity);
            }
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
        RefRW<BuildOrder> order = SystemAPI.GetSingletonRW<BuildOrder>(); //for some reason i need to get it every frame, otherwise null error
        order.ValueRW.classValue = BuildingType.None; 
    }
}