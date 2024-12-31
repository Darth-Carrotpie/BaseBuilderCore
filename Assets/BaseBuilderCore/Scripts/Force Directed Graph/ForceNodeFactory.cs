using BovineLabs.Core;
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

namespace BaseBuilderCore {
    [BurstCompile]
    public partial struct ForceNodeFactory : ISystem {
        EntityManager entityManager;
        RefRW<TestForceDirection> testConfig;
        //EntityCommandBuffer ecb;
        Entity configEntity;
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TestForceDirection>();
            state.RequireForUpdate<ForceDirGraphConfig>();
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }

        //build a query buildings without respective nodes:::::
        //Then create a node for each of those buildings
        public void OnUpdate(ref SystemState state) {
            //BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            //ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Entity orderEntity = entityManager.CreateEntityQuery(typeof(BuildOrder)).GetSingletonEntity();
            DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<BuildOrderAtPosition>(orderEntity);

            if (buildOrdersAtPos.Length <= 0) return;

            //create the building entity for each order: 
            for (int i = buildOrdersAtPos.Length - 1; i >= 0; i--) {
                BuildOrderAtPosition bo = buildOrdersAtPos[i];
                if (bo.buildingProduced == Entity.Null) return;
                if (bo.forceNodeProduced != Entity.Null) return;
                //var nm = entityManager.GetName(bo.buildingProduced);
                //UnityEngine.Debug.Log("CreateForceNodeAtPosition buildingRepr: "+ nm);
                Entity newNode = CreateForceNodeAtPosition(bo.buildingProduced, bo.position, bo.isFirst, ref state);

                //swap the order with new node
                BuildOrderAtPosition newBo = new BuildOrderAtPosition {
                    isFirst = bo.isFirst,
                    buildOrder = bo.buildOrder,
                    buildingProduced = bo.buildingProduced,
                    forceNodeProduced = newNode,
                    position = bo.position,
                };
                buildOrdersAtPos.RemoveAt(i);
                buildOrdersAtPos.Add(newBo);
            }

            //only reset this to zeroes when we add new nodes to iteration. Then we reset temperature and initial velocities.
            foreach (var (physicsVelocity, node) in SystemAPI.Query<RefRW<PhysicsVelocity>, ForceNode>()) {
                physicsVelocity.ValueRW.Linear = new float3(0, 0, 0);
            }
            RefRW<ForceDirGraphConfig> graphConfig = SystemAPI.GetSingletonRW<ForceDirGraphConfig>();
            graphConfig.ValueRW.temperature = graphConfig.ValueRW.initialTemperature;
        }

        /*public void CreateForceNode(Entity buildingEntity)
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
        }*/
        public Entity CreateForceNodeAtPosition(Entity buildingEntity, float3 position, bool isFirst, ref SystemState state) {
            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
            ForceDirGraphConfig config = SystemAPI.GetSingleton<ForceDirGraphConfig>();

            Entity newNode = entityManager.Instantiate(config.nodeEntityPrefab);
            ecb.SetComponent(newNode, new LocalTransform {
                Position = new float3(position.x, 0, position.z),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            ecb.SetComponent(newNode, new ForceNode { buildingRepr = buildingEntity });
            ecb.SetName(newNode, "ForceNode_x:" + position.x + "_z:" + position.z);

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
        struct ExtractBuildingReprJob : IJobParallelFor {
            [ReadOnly] public ComponentLookup<ForceNode> ForceNodes;
            [ReadOnly] public NativeArray<Entity> Entities;
            [WriteOnly] public NativeArray<Entity> BuildingReprArray;

            public void Execute(int index) {
                Entity entity = Entities[index];
                ForceNode forceNode = ForceNodes[entity];
                BuildingReprArray[index] = forceNode.buildingRepr;
            }
        }
    }
}