using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

namespace BaseBuilderCore {
    [BurstCompile]
    public partial struct BuildFactory : ISystem {
        EntityManager entityManager;
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<BuildOrder>();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            var ecbSystem = state.World.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer();
            Entity orderEntity = SystemAPI.GetSingletonEntity<BuildOrder>();
            DynamicBuffer<BuildOrderAtPosition> buildOrdersAtPos = SystemAPI.GetBuffer<BuildOrderAtPosition>(orderEntity);

            if (buildOrdersAtPos.Length <= 0) return;

            //create the building entity for each order:
            for (int i = buildOrdersAtPos.Length - 1; i >= 0; i--) {
                BuildOrderAtPosition bo = buildOrdersAtPos[i];
                if (bo.buildingProduced != Entity.Null) return;
                //fill in the order with new building 
                //Problem: this is not working. Building not being set!
                //Cause: the ecb is somehow deffering the instantiation of the building, after its playback the refs are lost.
                //solution: use entityManager.CreateEntity() instead of ecb.CreateEntity();
                Entity newBuildingEntity = entityManager.CreateEntity();

                ecb.AddComponent(newBuildingEntity, new Building { buildingType = bo.buildOrder.classValue });
                string bName = bo.buildOrder.classValue.ToString();
                ecb.SetName(newBuildingEntity, "New_" + bName);

                UnityEngine.Debug.Log("building created: " + bName);

                //add LocalToWorld to be able to parent later
                ecb.AddComponent<LocalToWorld>(newBuildingEntity);

                var newBo = bo;
                newBo.buildingProduced = newBuildingEntity;

                buildOrdersAtPos[i] = newBo;
            }
            for (int i = buildOrdersAtPos.Length - 1; i >= 0; i--) {
                UnityEngine.Debug.Log("buildOrdersAtPos[" + i + "]: " + buildOrdersAtPos[i].buildingProduced);
            }

            ecbSystem.AddJobHandleForProducer(state.Dependency);
        }
    }
}