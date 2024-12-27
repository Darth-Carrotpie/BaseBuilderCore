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
        NativeArray<Entity> orderArray = entityManager.CreateEntityQuery(typeof(DestroyOrder)).ToEntityArray(Allocator.Temp);
        if (orderArray.Length == 0) return;
        Entity orderEntity = orderArray[0];
        DestroyOrder buildOrdersAtPos = entityManager.GetComponentData<DestroyOrder>(orderEntity);

        //query all selected Entities
        NativeArray<Entity> entityArray = entityManager.CreateEntityQuery(typeof(LocalTransform), typeof(SelectedCellTag)).ToEntityArray(Allocator.TempJob);
        if (entityArray.Length == 0)
        {
            UnityEngine.Debug.Log("Nothing Selected! Nothing to destroy!");
            return;
        }
        UnityEngine.Debug.Log("Destroy order tag set to Enabled!");

        entityManager.SetComponentEnabled<DestroyOrder>(orderEntity, true);
    }
}
