using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct DestroyLinks : ISystem
{
    EntityManager entityManager;
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<TemplateComponent>();
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
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        Entity orderEntity = SystemAPI.GetSingletonEntity<BuildOrder>();
        DynamicBuffer<DestroyOrderAtPosition> destroyOrderAtPos = SystemAPI.GetBuffer<DestroyOrderAtPosition>(orderEntity);

        foreach ((ForceLink link, Entity linkEntity) in SystemAPI.Query<ForceLink>().WithEntityAccess())
        {
            foreach (var order in destroyOrderAtPos)
            {
                float3 linkPositionA = entityManager.GetComponentData<LocalToWorld>(link.nodeA).Position;
                float3 linkPositionB = entityManager.GetComponentData<LocalToWorld>(link.nodeB).Position;

                // Check if the order's position correlates with the link's positions
                if (math.distance(linkPositionA, order.position) < 0.1f ||
                    math.distance(linkPositionB, order.position) < 0.1f)
                {
                    // Destroy the link if it correlates with the order
                    ecb.AddComponent(linkEntity, new MarkedForDestruction { }); //this will tag to destroy entity later
                    //ecb.DestroyEntity(linkEntity);
                    UnityEngine.Debug.Log("Destroyed link between: " + link.nodeA + " and " + link.nodeB);
                }
            }
        }
    }
}
