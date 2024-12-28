using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct DestroyOrderTTLReducer : ISystem {
    //EntityManager entityManager;
    public void OnCreate(ref SystemState state) {
        //entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void OnStartRunning(ref SystemState state) {
    }
    public void OnDestroy(ref SystemState state) {
    }
    public void OnUpdate(ref SystemState state) {
        //BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        //var ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        Entity orderEntity = SystemAPI.GetSingletonEntity<DestroyOrder>();
        DynamicBuffer<DestroyOrderAtPosition> destroyOrderAtPos = SystemAPI.GetBuffer<DestroyOrderAtPosition>(orderEntity);

        for (int i = destroyOrderAtPos.Length - 1; i >= 0; i--) {
            DestroyOrderAtPosition bo = destroyOrderAtPos[i];
            // reduce TTL by one of each existing order
            var newBo = bo;
            newBo.TTL -= 1;
            destroyOrderAtPos[i] = newBo;
        }
    }
}