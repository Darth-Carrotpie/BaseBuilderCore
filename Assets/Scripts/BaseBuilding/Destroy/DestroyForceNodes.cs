using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct DestroyForceNodes : ISystem
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

        foreach ((LocalTransform localTransform, Entity entity) in SystemAPI.Query<LocalTransform>().WithEntityAccess())
        {
            foreach (var order in destroyOrderAtPos)
            {
                // Check if the entity's position correlates with the order's position
                if (math.distance(localTransform.Position, order.position) < 0.1f)
                {
                    // Destroy the ForceNode if it correlates with the order
                    ecb.AddComponent(entity, new MarkedForDestruction { }); //this will tag to destroy entity later
                    //ecb.DestroyEntity(entity);
                    UnityEngine.Debug.Log("Destroyed ForceNode at position: " + localTransform.Position);
                }
            }
        }
    }
}
