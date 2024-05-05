using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct TestForceDirectionSystem : ISystem
{
    EntityManager entityManager;

    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<LinkOrder>();
        state.RequireForUpdate<TestForceDirection>();
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    { 
    }

    public void OnUpdate(ref SystemState state)
    //protected override void OnUpdate() 
    {
        // First we get a reference to the Entity of config Singleton
        RefRW < TestForceDirection> testConfig = SystemAPI.GetSingletonRW<TestForceDirection>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        if (testConfig.ValueRW.generateNodes)
        {
            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
            for (int i = 0; i < testConfig.ValueRW.spawnAmount; i++)
            {
                CreateNodeWithRandomLink(ecb, testConfig);
            }
            testConfig.ValueRW.generateNodes = false;
        }
    }

    public void CreateNodeWithRandomLink(EntityCommandBuffer ecb, RefRW<TestForceDirection> testConfig)
    {
        Entity randNode = GetRandomNodeSystem();
        var entityQuery = entityManager.CreateEntityQuery(typeof(TestForceDirection));
        Entity configEntity = entityQuery.ToEntityArray(Allocator.TempJob)[0];

        var newNode = ecb.Instantiate(testConfig.ValueRW.nodeEntityPrefab);
        var newLink = ecb.Instantiate(testConfig.ValueRW.linkEntityPrefab);
        ecb.AddComponent<Parent>(newNode);
        ecb.SetComponent(newNode, new Parent { Value = configEntity });
        ecb.AddComponent<Parent>(newLink);
        ecb.SetComponent(newLink, new Parent { Value = configEntity });
        ecb.SetComponent(newNode, new LocalTransform
        {
            Position = new float3(UnityEngine.Random.Range(-10f, 5f), 0, UnityEngine.Random.Range(-5f, 10f)),
            Rotation = quaternion.identity,
            Scale = 1f
        });
        ecb.SetComponent(newLink, new ForceLink
        {
            nodeA = randNode,
            nodeB = newNode,
        });
    }

    public Entity GetRandomNodeSystem()
    {
        var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, int.MaxValue)); // Seed the random number generator
        Entity randomEntity = Entity.Null;

        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ForceNode>());
        // Get the array of entities with TestForceDirection component
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);
        int sizeEntities = entities.Length;
        // Check if there are any entities in the array
        if (entities.Length > 0)
        {
            // Generate a random index within the valid range of indices for the array
            int randomIndex = UnityEngine.Random.Range(0, entities.Length);

            // Get the random entity at the random index
            randomEntity = entities[randomIndex];
        }

        entities.Dispose();

        if (randomEntity == Entity.Null)
        {
            UnityEngine.Debug.LogWarning("No entities found with ForceNode component.");
        }
        return randomEntity;
    }
}