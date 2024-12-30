using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BaseBuilderCore {
    public partial struct DestroyForceNodes : ISystem {
        //EntityManager entityManager;
        public void OnCreate(ref SystemState state) {
        }

        [BurstCompile]
        public void OnStartRunning(ref SystemState state) {
            //entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }
        public void OnDestroy(ref SystemState state) {
        }
        public void OnUpdate(ref SystemState state) {
            BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

            Entity orderEntity = SystemAPI.GetSingletonEntity<DestroyOrder>();
            DynamicBuffer<DestroyOrderAtPosition> destroyOrderAtPos = SystemAPI.GetBuffer<DestroyOrderAtPosition>(orderEntity);

            //contains hex grid size:
            GridGeneratorConfig config = SystemAPI.GetSingleton<GridGeneratorConfig>();


            foreach ((LocalTransform localTransform, ForceNode node, DynamicBuffer<GridCellArea> dynBuffer, Entity nodeEntity) in SystemAPI.Query<LocalTransform, ForceNode, DynamicBuffer<GridCellArea>>().WithEntityAccess()) {
                for (int i = destroyOrderAtPos.Length - 1; i >= 0; i--) {
                    DestroyOrderAtPosition order = destroyOrderAtPos[i];
                    if (order.forceNodeDestroyed == true) continue;
                    if (order.forceLinkDestroyed == false) {

                        //skip this if the node still has any links
                        if (GetLinksAmountForNode(nodeEntity, ref state) != 0) {
                            continue;
                        }
                    }

                    //check their ref to current grid cell
                    GridCellArea gca = dynBuffer.AsNativeArray().FirstOrDefault();
                    Entity gridCellEntity = gca.GridCellEntity;
                    //check gridCellEntity position instead of node position!
                    LocalTransform gridCellTransform = SystemAPI.GetComponent<LocalTransform>(gridCellEntity);

                    // Check if the entity's position correlates with the order's position
                    //the problem is with positioning, should instead get the closest node somehow more deterministic!
                    if (math.distance(gridCellTransform.Position, order.position) < config.hexRadius / 2f) {
                        // Destroy the ForceNode if it correlates with the order
                        ecb.AddComponent(nodeEntity, new MarkedForDestruction { }); //this will tag to destroy entity later
                                                                                    // Put tag for destruction to the data layer
                        ecb.AddComponent(node.buildingRepr, new MarkedForDestruction { }); //this will tag to destroy entity later
                                                                                           //ecb.DestroyEntity(entity);
                        UnityEngine.Debug.Log("Destroyed ForceNode at position: " + localTransform.Position);

                        //change order state to reflect the destruction of force node
                        var newDo = order;
                        newDo.forceNodeDestroyed = true;
                        destroyOrderAtPos[i] = newDo;
                    }
                }
            }
        }

        int GetLinksAmountForNode(Entity nodeEntity, ref SystemState state) {
            int amount = 0;
            foreach (ForceLink link in SystemAPI.Query<ForceLink>()) {
                if (link.nodeA == nodeEntity || link.nodeB == nodeEntity) {
                    amount++;
                }
            }
            return amount;
        }
    }
}