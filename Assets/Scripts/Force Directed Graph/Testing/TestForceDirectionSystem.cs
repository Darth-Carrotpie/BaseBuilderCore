using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
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
    RefRW<TestForceDirection> testConfig;
    EntityCommandBuffer ecb;
    Entity configEntity;
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
    {
        // First we get a reference to the Entity of config Singleton
        testConfig = SystemAPI.GetSingletonRW<TestForceDirection>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        if (testConfig.ValueRW.generateNodes)
        {
            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
            for (int i = 0; i < testConfig.ValueRW.spawnAmount; i++)
            {
                CreateNodeWithRandomLinks(testConfig.ValueRW.linkAmount);
            }
            testConfig.ValueRW.generateNodes = false;

            //only reset this to zeroes when we add new nodes to iteration. Then we reset temperature and initial velocities.
            foreach (var (physicsVelocity, node) in SystemAPI.Query<RefRW<PhysicsVelocity>, ForceNode>())
            {
                physicsVelocity.ValueRW.Linear = new float3(0, 0, 0);
            }
        }
    }

    public void CreateNodeWithRandomLinks(int linksAmount)
    {
        var entityQuery = entityManager.CreateEntityQuery(typeof(TestForceDirection));
        configEntity = entityQuery.ToEntityArray(Allocator.TempJob)[0]; 

        var newNode = ecb.Instantiate(testConfig.ValueRW.nodeEntityPrefab);
        ecb.AddComponent<Parent>(newNode);
        ecb.SetComponent(newNode, new Parent { Value = configEntity });
        ecb.SetComponent(newNode, new LocalTransform
        {
            Position = new float3(UnityEngine.Random.Range(-10f, 5f), 0, UnityEngine.Random.Range(-5f, 10f)),
            Rotation = quaternion.identity,
            Scale = 1f
        });
        for (int i = 0;i < linksAmount;i++)
        {
            MakeRandomLink(newNode);
        }
    }

    //this doest not properly exclude test links randomization of the picked node to link randomly. In such a case it just skips to create the link.
    //To Do: rework to be deterministic and always return a particular random number of connections.
    public void MakeRandomLink(Entity newNode)
    {
        Entity randNode = GetRandomNodeSystem();
        if (randNode == newNode) return;
        var newLink = ecb.Instantiate(testConfig.ValueRW.linkEntityPrefab);
        ecb.AddComponent<Parent>(newLink);
        ecb.SetComponent(newLink, new Parent { Value = configEntity });
        ecb.SetComponent(newLink, new ForceLink
        {
            nodeA = randNode,
            nodeB = newNode,
        });
    }

    public Entity GetRandomNodeSystem()
    {
        Entity randomEntity = Entity.Null;
        EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly<ForceNode>());

        // Get the array of entities with TestForceDirection component
        NativeArray<Entity> entities = query.ToEntityArray(Allocator.TempJob);
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