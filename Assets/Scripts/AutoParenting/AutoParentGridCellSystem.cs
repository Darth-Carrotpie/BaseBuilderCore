using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Search;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

//[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct AutoParentGridCellSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        //state.RequireForUpdate<BuildOrder>();
    }
    public void OnStartRunning(ref SystemState state)
    {
    }
    public void OnDestroy(ref SystemState state)
    {
    }
    public void OnUpdate(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton begSimEcb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>(); //use BeginSimulationEntityCommandBufferSystem because otherwise it will render before applying position
        EntityCommandBuffer ecb = begSimEcb.CreateCommandBuffer(state.WorldUnmanaged);
        var singletonQuery = SystemAPI.QueryBuilder().WithAll<HexCellParent>().Build();
        if (singletonQuery.IsEmpty) return;
        Entity parentEntity = singletonQuery.GetSingletonEntity();

        foreach ((RefRW<GridCell> gridCell, Entity entity) in SystemAPI.Query<RefRW<GridCell>>().WithNone<Parent>().WithNone<PreviousParent>().WithNone<ExcludeFromAutoParenting>().WithEntityAccess())
        {
            ecb.AddComponent<Parent>(entity);
            ecb.SetComponent(entity, new Parent { Value = parentEntity });
        }
    }
}