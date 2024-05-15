using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[BurstCompile]
public partial struct AutoParentingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BuildOrder>();
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((RefRW<Building> gridCell, Entity entity) in SystemAPI.Query<RefRW<Building>>().WithAbsent<Parent>().WithEntityAccess())
        {
            ecb.AddComponent<Parent>(entity);
            Entity parentEntity = CreateParentEntityIfNotExists<BuildingsParent>(ecb, state);
            ecb.SetComponent(entity, new Parent { Value = parentEntity });
        }
    }

    Entity CreateParentEntityIfNotExists<T>(EntityCommandBuffer ecb, SystemState state) where T : struct, IComponentData
        //To Do:
        // : struct, IComponentData should have solved the error, but hasn't.
    {
        var singletonQuery = state.GetEntityQuery(ComponentType.ReadOnly<T>());
        if (singletonQuery.IsEmpty)
        {
            Entity newParentEntity = ecb.CreateEntity();
            ecb.AddComponent<BuildingsParent>(newParentEntity); //should be type T, but gives non-nullable error
            ecb.SetName(newParentEntity, typeof(T).Name);
            return newParentEntity;
        }
        return singletonQuery.GetSingletonEntity();
    }
}