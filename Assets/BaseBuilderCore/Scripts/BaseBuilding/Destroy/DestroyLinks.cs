using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BaseBuilderCore {
    public partial struct DestroyLinks : ISystem {
        EntityManager entityManager;
        public void OnCreate(ref SystemState state) {
            //state.RequireForUpdate<TemplateComponent>();
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        public void OnStartRunning(ref SystemState state) {
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

            Entity orderEntity = SystemAPI.GetSingletonEntity<BuildOrder>();
            DynamicBuffer<DestroyOrderAtPosition> destroyOrderAtPos = SystemAPI.GetBuffer<DestroyOrderAtPosition>(orderEntity);
            GridGeneratorConfig config = SystemAPI.GetSingleton<GridGeneratorConfig>();
            for (int i = destroyOrderAtPos.Length - 1; i >= 0; i--) {
                DestroyOrderAtPosition order = destroyOrderAtPos[i];
                //skip this order if the links for it are already destroyed
                if (order.forceLinkDestroyed == true) continue;

                foreach ((ForceLink link, Entity linkEntity) in SystemAPI.Query<ForceLink>().WithEntityAccess()) {
                    Entity hexA = GetNodeCell(link.nodeA);
                    Entity hexB = GetNodeCell(link.nodeB);

                    float3 linkPositionA = entityManager.GetComponentData<LocalTransform>(hexA).Position;
                    float3 linkPositionB = entityManager.GetComponentData<LocalTransform>(hexB).Position;

                    // Check if the order's position correlates with the link's positions
                    if (math.distance(linkPositionA, order.position) < config.hexRadius ||
                        math.distance(linkPositionB, order.position) < config.hexRadius) {
                        // Destroy the link if it correlates with the order
                        ecb.AddComponent(linkEntity, new MarkedForDestruction { }); //this will tag to destroy entity later

                        //change order state to reflect the destruction of force node
                        var newDo = order;
                        newDo.forceLinkDestroyed = true;
                        destroyOrderAtPos[i] = newDo;
                    }
                }
            }
        }

        Entity GetNodeCell(Entity nodeEntity) {
            DynamicBuffer<GridCellArea> dynBuffer = entityManager.GetBuffer<GridCellArea>(nodeEntity);
            GridCellArea gca = dynBuffer.AsNativeArray().FirstOrDefault();
            return gca.GridCellEntity;
        }
    }
}