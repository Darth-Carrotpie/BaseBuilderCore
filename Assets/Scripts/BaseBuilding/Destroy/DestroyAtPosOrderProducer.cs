using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct DestroyAtPosOrderProducer : ISystem
{
    EntityManager entityManager;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Get required references
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        NativeArray<Entity> orderArray = entityManager.CreateEntityQuery(typeof(DestroyOrder)).ToEntityArray(Allocator.Temp);
        if (orderArray.Length == 0) return;
        Entity orderEntity = orderArray[0];
        UnityEngine.Debug.Log("Found a DestroyOrder!");

        //Find out if particular order is enabled
        if (!entityManager.IsComponentEnabled<DestroyOrder>(orderEntity)) return;

        entityManager.SetComponentEnabled<DestroyOrder>(orderEntity, false);

        //Get the buffer, where we will add OrderAtPosition components
        DynamicBuffer<DestroyOrderAtPosition> buildOrdersAtPos = entityManager.GetBuffer<DestroyOrderAtPosition>(orderEntity);
        UnityEngine.Debug.Log("DestroyOrder is enabled, add OrdersAtPos!");

        //create DestroyOrderAtPosition for each selection
        foreach ((var localTransform, var cell, var gridCell, Entity gridCellEntity) in SystemAPI.Query<LocalTransform, SelectableCellTag, GridCell>().WithAll<SelectedCellTag>().WithEntityAccess())
        {
            //create DestroyOrderAtPosition for each selection
            DestroyOrderAtPosition newDOatPosition = new DestroyOrderAtPosition
            {
                position = localTransform.Position,
                buildingDestroyed = false,
                forceNodeDestroyed = false,
                forceLinkDestroyed = false,
                TTL = 25
            };
            buildOrdersAtPos.Add(newDOatPosition);
        }
    }
}
