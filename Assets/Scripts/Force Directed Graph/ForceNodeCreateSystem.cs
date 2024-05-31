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
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();
        DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);

        if (buildOrdersAtPos.Length <= 0) return;

        //create the building entity for each order: 
        for (int i = buildOrdersAtPos.Length - 1; i >= 0; i--)
        {
            BuildOrderAtPosition bo = buildOrdersAtPos[i];
            if (bo.buildingProduced == Entity.Null) return;
            if (bo.forceNodeProduced != Entity.Null) return;
            Entity newNode = CreateForceNodeAtPosition(bo.buildingProduced, bo.position);

            //swap the order with new node
            BuildOrderAtPosition newBo = new BuildOrderAtPosition
            {
                buildOrder = bo.buildOrder,
                buildingProduced = bo.buildingProduced,
                forceNodeProduced = newNode,
                position = bo.position,
            };
            buildOrdersAtPos.RemoveAt(i);
            buildOrdersAtPos.Add(newBo);
        }
        /*
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
        */
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
        //ecb.AddComponent<Parent>(newNode);
        //ecb.SetComponent(newNode, new Parent { Value = configEntity });
        ecb.SetComponent(newNode, new LocalTransform
        {
            Position = new float3(UnityEngine.Random.Range(-3f, 3f), 0, UnityEngine.Random.Range(-3f, 3f)),
            Rotation = quaternion.identity,
            Scale = 1f
        });
        ecb.SetComponent(newNode, new ForceNode { buildingRepr = buildingEntity });
    }
    public Entity CreateForceNodeAtPosition(Entity buildingEntity, float3 position)
    {
        var entityQuery = entityManager.CreateEntityQuery(typeof(ForceDirGraphConfig));
        configEntity = entityQuery.ToEntityArray(Allocator.TempJob)[0];

        var config = SystemAPI.GetSingleton<ForceDirGraphConfig>();

        var newNode = ecb.Instantiate(config.nodeEntityPrefab);
        //ecb.AddComponent<Parent>(newNode);
        //ecb.SetComponent(newNode, new Parent { Value = configEntity });
        ecb.SetComponent(newNode, new LocalTransform
        {
            Position = new float3(position.x, 0, position.z),
            Rotation = quaternion.identity,
            Scale = 1f
        });
        ecb.SetComponent(newNode, new ForceNode { buildingRepr = buildingEntity });
        return newNode;
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