using BovineLabs.Core.States;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace BaseBuilderCore {
    public partial struct DestroyAtPosOrderProducer : ISystem {
        EntityManager entityManager;
        [BurstCompile]
        public void OnStartRunning(ref SystemState state) {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            //Get required references
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            RefRW<DestroyOrder> order = SystemAPI.GetSingletonRW<DestroyOrder>();
            Entity orderEntity = entityManager.CreateEntityQuery(typeof(DestroyOrder)).GetSingletonEntity();

            //Find out if particular order is enabled
            if (order.ValueRW.Value == false) return;

            order.ValueRW.Value = false;

            //Get the buffer, where we will add DestroyOrderAtPosition components
            DynamicBuffer<DestroyOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<DestroyOrderAtPosition>(orderEntity);
            UnityEngine.Debug.Log("DestroyOrder is enabled, add OrdersAtPos!");

            //create DestroyOrderAtPosition for each selection
            foreach ((var localTransform, var cell, var gridCell, Entity gridCellEntity) in SystemAPI.Query<LocalTransform, SelectableCellTag, GridCell>().WithAll<SelectedCellTag>().WithEntityAccess()) {
                //create DestroyOrderAtPosition for each selection
                DestroyOrderAtPosition newDOatPosition = new DestroyOrderAtPosition {
                    position = localTransform.Position,
                    buildingDestroyed = false,
                    forceNodeDestroyed = false,
                    forceLinkDestroyed = false,
                    TTL = 25
                };
                buildOrdersAtPos.Add(newDOatPosition);
                //deselect the cell
                entityManager.SetComponentEnabled<SelectedCellTag>(gridCellEntity, false);
            }
        }
    }
}
