using System;
using System.Diagnostics;
using System.Net.Security;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct ForceNodeCreateSystem : ISystem
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

        //build a query buildings without respective nodes:::::
        //Then create a node for each of those buildings
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        // First we get a reference to the Entity of config Singleton
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //first build a query with all building and make a NativeArray
        var buildingsQuery = SystemAPI.QueryBuilder().WithAll<Building>().WithNone<ExcludeFromAutoParenting>().Build();
        int bCount = buildingsQuery.CalculateEntityCount();
        NativeArray<Entity> buildingArray = buildingsQuery.ToEntityArray(Allocator.TempJob);

        //then query all nodes and put their building refs into an array
        var nodesQuery = SystemAPI.QueryBuilder().WithAll<ForceNode>().WithNone<ExcludeFromAutoParenting>().Build();
        int nCount = nodesQuery.CalculateEntityCount();
        NativeArray<Entity> buildingReprArray = new NativeArray<Entity>(nCount, Allocator.TempJob);

        var extractJob = new ExtractBuildingReprJob
        {
            ForceNodes = SystemAPI.GetComponentLookup<ForceNode>(true),
            Entities = nodesQuery.ToEntityArray(Allocator.TempJob),
            BuildingReprArray = buildingReprArray
        };

        // Schedule the job and complete it
        JobHandle nHandle = extractJob.Schedule(nCount, 64);
        nHandle.Complete();

        // Find the difference between buildingArray and buildingReprArray
        NativeList<Entity> diffArray = new NativeList<Entity>(Allocator.TempJob);

        for (int i = 0; i < buildingArray.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < buildingReprArray.Length; j++)
            {
                if (buildingArray[i] == buildingReprArray[j])
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                diffArray.Add(buildingArray[i]);
            }
        }
        //query buildings with links

        //create ForceNodes
        foreach(Entity buildingEnt in diffArray)
        {
            CreateForceNode(buildingEnt);
        }

        buildingArray.Dispose();
        buildingReprArray.Dispose();
        extractJob.Entities.Dispose();
        diffArray.Dispose();

        //only reset this to zeroes when we add new nodes to iteration. Then we reset temperature and initial velocities.
        foreach (var (physicsVelocity, node) in SystemAPI.Query<RefRW<PhysicsVelocity>, ForceNode>())
        {
            physicsVelocity.ValueRW.Linear = new float3(0, 0, 0);
        }
    }

    public void CreateForceNode(Entity buildingEntity)
    {
        var entityQuery = entityManager.CreateEntityQuery(typeof(ForceDirGraphConfig));
        configEntity = entityQuery.ToEntityArray(Allocator.TempJob)[0];

        var config = SystemAPI.GetSingleton<ForceDirGraphConfig>();

        var newNode = ecb.Instantiate(config.nodeEntityPrefab);
        ecb.AddComponent<Parent>(newNode);
        ecb.SetComponent(newNode, new Parent { Value = configEntity });
        ecb.SetComponent(newNode, new LocalTransform
        {
            Position = new float3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f)),
            Rotation = quaternion.identity,
            Scale = 1f
        });
        ecb.SetComponent(newNode, new ForceNode { buildingRepr = buildingEntity });
    }

    /*public void CreateNodeWithRandomLinks(int linksAmount)
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
        for (int i = 0; i < linksAmount; i++)
        {
            MakeRandomLink(newNode);
        }
    }*/

    [BurstCompile]
    struct ExtractBuildingReprJob : IJobParallelFor
    {
        [ReadOnly] public ComponentLookup<ForceNode> ForceNodes;
        [ReadOnly] public NativeArray<Entity> Entities;
        [WriteOnly] public NativeArray<Entity> BuildingReprArray;

        public void Execute(int index)
        {
            Entity entity = Entities[index];
            ForceNode forceNode = ForceNodes[entity];
            BuildingReprArray[index] = forceNode.buildingRepr;
        }
    }
}