using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Profiling;

public class DestroyOrderInput : MonoBehaviour
{
    EntityManager entityManager;
    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void DestroySelectedNodes()
    {

        Entity orderEntity = entityManager.CreateEntityQuery(typeof(DestroyOrder)).GetSingletonEntity();
        DestroyOrder orderData = entityManager.GetComponentData<DestroyOrder>(orderEntity);
        //check if order already exists
        if (orderData.Value != false) return;

        //check if nothing is selected
        NativeArray<Entity> entityArray = entityManager.CreateEntityQuery(typeof(LocalTransform), typeof(SelectedCellTag)).ToEntityArray(Allocator.TempJob);
        if (entityArray.Length == 0)
        {
            UnityEngine.Debug.Log("Nothing Selected! Nothing to destroy!");
            return;
        }

        DestroyOrder newOrder = new DestroyOrder {
            Value = true,
        };

        entityManager.AddComponentData<DestroyOrder>(orderEntity, newOrder);
    }
}
